using Contest.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

                Log.Info($"Considering {targets.Count:N0} potential taragets");

                // sort targets based on potential
                targets = targets.OrderBy(x => x.MLen - x.NearbyTargets.Count).ToList();

                var pf = new AstarMatrixPather(system.Matrix);
                BotTarget target = null;

                foreach (var t in targets)
                {
                    Log.Info($"Examining {t.Coordinate} ({t.NearbyTargets.Count:N0} potential fills)");

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
                        Log.Info($"Making sure fill {fill} does not block us in");
                        testSystem.CmdFill(1, t.Coordinate.GetDifference(fill));
                        var testPf = new AstarMatrixPather(testSystem.Matrix);
                        var (_, commands) = testPf.GetRouteTo(t.Coordinate, Coordinate.Zero);
                        if (commands == null)
                        {
                            Log.Info($"Removed {fill}");
                            t.NearbyTargets.Remove(fill);
                        }
                    }

                    if (t.NearbyTargets.Count == 0)
                        continue;

                    target = t;
                    break;
                }

                Log.Info($"Target selected: {target.Coordinate} ({target.NearbyTargets.Count} nearby targets)");

                // move to it
                system.ExecuteCommands(target.Commands);

                // fill it
                foreach (var fc in target.NearbyTargets)
                {
                    var dv = fc.GetDifference(target.Coordinate);
                    system.CmdFill(1, dv);
                    voxels.Remove(fc);
                }

                Log.Info($"{voxels.Count:N0} target voxels left");

                MdlFile.SaveModel("test.mdl", system.Matrix);
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
            var sw = Stopwatch.StartNew();

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

            Log.Debug($"Calculated {list.Count:N0} target routes in {sw.ElapsedMilliseconds:N0} ms");

            return list.ToList();
        }
    }
}