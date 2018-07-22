//using Contest.Core;
//using Contest.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace Contest.Controllers
//{
//    public class Solver
//    {
//        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        public void Solve(string filename, string outputFile)
//        {
//            Log.Info($"Solving file {filename}");

//            // load target
//            var targetMatrix = MdlFile.LoadModel(filename);

//            // create system
//            var system = new MatterSystem(targetMatrix.Resolution);

//            var voxels = GetRemainingBlocks(system.Matrix, targetMatrix);

//            Log.Info($"{voxels.Count:N0} total voxels to fill");
//            int start = voxels.Count;
//            int lastComplete = 0;

//            // while (! done)
//            while (targetMatrix != system.Matrix)
//            {
//                var searchSpace = GenerateSearchSpace(voxels, system.Matrix);
//                //Log.Info($"Searching space of {searchSpace.Count:N0} voxels");
//                //Log.Info($"Bot is at {system.Bots[1].Position}");

//                var targets = BuildTargetCoordStats(system, searchSpace, voxels);

//                //Log.Info($"Considering {targets.Count:N0} potential targets");

//                // sort targets based on potential
//                targets = targets.OrderBy(x => x.NearbyTargets.Min(t => t.Y))
//                    .ThenByDescending(x => x.NearbyTargets.Count(t => t.Y == x.NearbyTargets.Min(y => y.Y)))
//                    .ThenBy(x => system.Bots[1].Position.Mlen(x.Coordinate)).ToList();

//                var pf = new AstarMatrixPather(system.Matrix);
//                BotTarget target = null;

//                foreach (var t in targets)
//                {
//                    //Log.Info($"Examining {t.Coordinate} ({t.NearbyTargets.Count:N0} potential fills)");

//                    // make sure we can nav there
//                    (_, t.Commands) = pf.GetRouteTo(system.Bots[1].Position, t.Coordinate);

//                    if (t.Commands == null)
//                    {
//                        Log.Info($"Could not navigate to {t.Coordinate}");
//                        continue;
//                    }

//                    // filter out fills that aren't on the lowest level
//                    t.NearbyTargets = t.NearbyTargets.Where(x => x.Y == t.NearbyTargets.Min(y => y.Y)).ToList();

//                    // check fills to make sure they don't block anything
//                    var possibleFills = t.NearbyTargets.Select(x => x).ToList();
//                    var testSystem = new MatterSystem(system);
//                    testSystem.ExecuteCommands(t.Commands);

//                    foreach (var fill in possibleFills)
//                    {
//                        var dv = fill.GetDifference(t.Coordinate);
//                        var cmd = new CommandFill(dv);
//                        testSystem.ExecuteCommand(1, cmd);

//                        var testPf = new AstarMatrixPather(testSystem.Matrix);
//                        var (_, commands) = testPf.GetRouteTo(testSystem.Bots[1].Position, Coordinate.Zero);
//                        if (commands == null)
//                        {
//                            t.NearbyTargets.Remove(fill);
//                            testSystem.Matrix.Unset(fill.X, fill.Y, fill.Z);
//                            continue;
//                        }

//                        //bool remove = false;

//                        ////foreach (var rv in voxels)
//                        //Parallel.ForEach(voxels, rv =>
//                        //{
//                        //    if (remove)
//                        //        return;

//                        //    if (t.NearbyTargets.Contains(rv))
//                        //        return;

//                        //    var (_, c) = testPf.GetRouteTo(testSystem.Bots[1].Position, rv);
//                        //    if (c == null)
//                        //    {
//                        //        remove = true;
//                        //    }
//                        //});

//                        //if (remove)
//                        //{
//                        //    t.NearbyTargets.Remove(fill);
//                        //    testSystem.Matrix.Unset(fill.x, fill.y, fill.z);
//                        //}
//                    }

//                    if (t.NearbyTargets.Count == 0)
//                        continue;

//                    target = t;
//                    break;
//                }

//                if (target != null)
//                {
//                    //Log.Info($"Target selected: {target.Coordinate} ({target.NearbyTargets.Count} nearby targets)");

//                    // move to it
//                    var cc = CompressCommands(target.Commands);
//                    system.ExecuteCommands(cc);

//                    // fill it
//                    foreach (var fc in target.NearbyTargets)
//                    {
//                        var dv = fc.GetDifference(target.Coordinate);
//                        var cmd = new CommandFill(dv);
//                        system.ExecuteCommand(1, cmd);
//                        voxels.Remove(fc);
//                    }
//                }
//                else
//                {
//                    // navigate home
//                    var bleh = new AstarMatrixPather(system.Matrix);
//                    var (_, zc) = bleh.GetRouteTo(system.Bots[1].Position, Coordinate.Zero);

//                    if (zc == null)
//                    {
//                        Log.Error("Could not nav home");
//                        throw new Exception();
//                    }

//                    system.ExecuteCommands(zc);

//                    // halt
//                    system.ExecuteCommand(1, new CommandHalt());
//                    targetMatrix = system.Matrix;
//                    TraceFile.WriteTraceFile(outputFile, system.Trace);
//                    Log.Error($"{Path.GetFileName(filename)} finished");
//                }

//                int complete = (int)(100 - ((float)voxels.Count / start * 100));
//                if (complete != lastComplete)
//                {
//                    lastComplete = complete;
//                    Log.Info($"{Path.GetFileName(filename)} {100 - ((float)voxels.Count / start * 100):N0}% complete");
//                }

//                //MdlFile.SaveModel("test.mdl", system.Matrix);
//                //TraceFile.WriteTraceFile("test.nbt", system.Trace);
//                //TraceFile.WriteTraceFile(outputFile, system.Trace);
//            }

//            // finish trace
//        }

//        private List<Command> CompressCommands(List<Command> commands)
//        {
//            if (commands.Count < 2)
//                return commands;

//            var l = new List<Command>();

//            for (int i = 0; i < commands.Count; i++)
//            {
//                if (commands[i] is CommandSmove c1 && i + 1 < commands.Count && commands[i + 1] is CommandSmove c2)
//                {
//                    if (c1.d.IsShortLinear && c2.d.IsShortLinear)
//                    {
//                        l.Add(new CommandLmove(c1.d, c2.d));
//                        i++;
//                        continue;
//                    }
//                }

//                l.Add(commands[i]);
//            }

//            return l;
//        }

//        // generate list of blocks
//        private HashSet<Coordinate> GetRemainingBlocks(Matrix src, Matrix target)
//        {
//            HashSet<Coordinate> coords = new HashSet<Coordinate>();
//            for (byte x = 0; x < src.Resolution; x++)
//                for (byte y = 0; y < src.Resolution; y++)
//                    for (byte z = 0; z < src.Resolution; z++)
//                        if (!src.Get(x, y, z) && target.Get(x, y, z))
//                            coords.Add(new Coordinate(x, y, z));

//            return coords;
//        }

//        private List<Coordinate> GenerateSearchSpace(HashSet<Coordinate> targets, Matrix src)
//        {
//            HashSet<Coordinate> space = new HashSet<Coordinate>();

//            foreach (var c in targets.Where(src.IsGrounded))
//            {
//                foreach (var nb in src.GetValidNearbies(c))
//                    space.Add(nb);
//            }

//            return space.ToList();
//        }

//        private List<BotTarget> BuildTargetCoordStats(MatterSystem src, List<Coordinate> space, HashSet<Coordinate> targets)
//        {
//            var list = new List<BotTarget>();

//            foreach (var c in space)
//            {
//                // get voxels that can be filled from here
//                var potentialFills = src.Matrix.GetValidNearbies(c).Where(targets.Contains).Where(src.Matrix.IsGrounded).ToList();

//                var cs = new BotTarget(c);
//                cs.NearbyTargets = potentialFills;
//                list.Add(cs);
//            }

//            return list;
//        }
//    }
//}