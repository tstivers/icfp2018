using Contest.Core;
using Contest.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contest.Controllers
{
    public class Solver
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Solve(string filename)
        {
            Log.Info($"Solving file {filename}");

            // load target
            var targetMatrix = MdlFile.LoadModel(filename);

            // create system
            var system = new MatterSystem(targetMatrix.Resolution);

            var voxels = GetRemainingBlocks(system.Matrix, targetMatrix);

            Log.Info($"{voxels.Count:N0} total voxels to fill");

            // while (! done)
            while (targetMatrix != system.Matrix)
            {
                var searchSpace = GenerateSearchSpace(voxels, system.Matrix);
                Log.Info($"Searching space of {searchSpace.Count:N0} voxels");
                Log.Info($"Bot is at {system.Bots[1].Position}");

                var targets = BuildTargetCoordStats(system, searchSpace, voxels);

                Log.Info($"Considering {targets.Count:N0} potential targets");

                // sort targets based on potential
                targets = targets.OrderBy(x => x.Coordinate.y).ThenByDescending(x => x.NearbyTargets.Count).ThenBy(x => x.MLen).ToList();

                var pf = new AstarMatrixPather(system.Matrix);
                BotTarget target = null;

                foreach (var t in targets)
                {
                    //Log.Info($"Examining {t.Coordinate} ({t.NearbyTargets.Count:N0} potential fills)");

                    // make sure we can nav there
                    (_, t.Commands) = pf.GetRouteTo(system.Bots[1].Position, t.Coordinate);

                    if (t.Commands == null)
                    {
                        Log.Info($"Could not navigate to {t.Coordinate}");
                        continue;
                    }

                    // check each fill to make sure we can still nav back to 0,0
                    var possibleFills = t.NearbyTargets.Select(x => x).ToList();
                    var testSystem = new MatterSystem(system);
                    testSystem.ExecuteCommands(t.Commands);

                    foreach (var fill in possibleFills)
                    {
                        var dv = fill.GetDifference(t.Coordinate);
                        var cmd = new CommandFill(dv);
                        testSystem.ExecuteCommand(1, cmd);
                    }

                    var testPf = new AstarMatrixPather(testSystem.Matrix);
                    var (_, commands) = testPf.GetRouteTo(testSystem.Bots[1].Position, Coordinate.Zero);
                    if (commands == null)
                    {
                        //Log.Info($"Possible blockage detected");
                        continue;
                    }

                    foreach (var rv in voxels)
                    {
                        if (t.NearbyTargets.Contains(rv))
                            continue;

                        var (_, c) = testPf.GetRouteTo(testSystem.Bots[1].Position, rv);
                        if (c == null)
                        {
                            Log.Info($"Possible voxel blockage detected");
                            t.NearbyTargets.Clear();
                        }
                    }

                    if (t.NearbyTargets.Count == 0)
                        continue;

                    target = t;
                    break;
                }

                if (target != null)
                {
                    Log.Info($"Target selected: {target.Coordinate} ({target.NearbyTargets.Count} nearby targets)");

                    // move to it
                    system.ExecuteCommands(target.Commands);

                    // fill it
                    foreach (var fc in target.NearbyTargets)
                    {
                        var dv = fc.GetDifference(target.Coordinate);
                        var cmd = new CommandFill(dv);
                        system.ExecuteCommand(1, cmd);
                        voxels.Remove(fc);
                    }
                }
                else
                {
                    // navigate home
                    var bleh = new AstarMatrixPather(system.Matrix);
                    var (_, zc) = bleh.GetRouteTo(system.Bots[1].Position, Coordinate.Zero);

                    if (zc == null)
                    {
                        Log.Error("Could not nav home");
                        throw new Exception();
                    }

                    system.ExecuteCommands(zc);

                    // halt
                    system.ExecuteCommand(1, new CommandHalt());
                    targetMatrix = system.Matrix;
                }

                Log.Info($"{voxels.Count:N0} target voxels left");

                MdlFile.SaveModel("test.mdl", system.Matrix);
                TraceFile.WriteTraceFile("test.nbt", system.Trace);
            }

            // finish trace
        }

        // generate list of blocks
        private HashSet<Coordinate> GetRemainingBlocks(Matrix src, Matrix target)
        {
            HashSet<Coordinate> coords = new HashSet<Coordinate>();
            for (byte x = 0; x < src.Resolution; x++)
                for (byte y = 0; y < src.Resolution; y++)
                    for (byte z = 0; z < src.Resolution; z++)
                        if (!src.Get(x, y, z) && target.Get(x, y, z))
                            coords.Add(new Coordinate(x, y, z));

            return coords;
        }

        private List<Coordinate> GenerateSearchSpace(HashSet<Coordinate> targets, Matrix src)
        {
            HashSet<Coordinate> space = new HashSet<Coordinate>();

            foreach (var c in targets.Where(src.IsGrounded))
            {
                foreach (var nb in src.GetValidNearbies(c))
                    space.Add(nb);
            }

            return space.ToList();
        }

        private List<BotTarget> BuildTargetCoordStats(MatterSystem src, List<Coordinate> space, HashSet<Coordinate> targets)
        {
            var list = new ConcurrentBag<BotTarget>();

            foreach (var c in space)
            {
                // get voxels that can be filled from here
                var potentialFills = src.Matrix.GetValidNearbies(c);
                potentialFills = potentialFills.Where(targets.Contains).ToList();
                potentialFills = potentialFills.Where(src.Matrix.IsGrounded).ToList();

                var cs = new BotTarget(c);
                cs.NearbyTargets = potentialFills;
                cs.MLen = c.Mlen(src.Bots[1].Position);
                list.Add(cs);
            }

            return list.ToList();
        }
    }
}