using Contest.Core;
using Contest.Core.Helpers;
using Contest.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Contest.Controllers
{
    public class Solver
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Solve(string targetFile, string sourceFile, string outputFile)
        {
            Log.Debug($"{Path.GetFileNameWithoutExtension(outputFile)} Starting solution");

            var isDecompiling = sourceFile != null && targetFile == null;

            // load target
            var targetData = (targetFile != null ? MdlFile.LoadModel(targetFile) : ((byte, BitArray)?)null);

            // load source
            var sourceData = (sourceFile != null ? MdlFile.LoadModel(sourceFile) : ((byte, BitArray)?)null);

            // create system
            var system = new MatterSystem(targetData?.Item1 ?? sourceData?.Item1 ?? 0, targetData?.Item2, sourceData?.Item2);

            Log.Info($"{Path.GetFileNameWithoutExtension(outputFile)} Created system");

            // get all the target voxels
            var targets = system.Matrix.Storage.Where(x => x.Filled != x.Target).ToList();
            var startCount = targets.Count;
            int lastComplete = 0;

            Log.Info($"{Path.GetFileNameWithoutExtension(outputFile)} Total targets: {targets.Count:N0}");

            while (targets.Count != 0)
            {
                IEnumerable<Voxel> targetVoxels;

                if (!isDecompiling)
                {
                    // pick the lowest closest target with the most changes to fill
                    targetVoxels = targets.Where(x => x.Grounded)
                        .OrderBy(x => x.Y)
                        .ThenByDescending(x => x.Nearby.Count(y => y.Filled != y.Target && y.Grounded))
                        .ThenBy(x => x.Mlen(system.Bots[1].Position));
                }
                else
                {
                    // pick the highest closest target with the most changes to void that doesn't unground anything
                    targetVoxels = targets.Where(x => x.Grounded)
                        .OrderByDescending(x => x.Y)
                        .ThenBy(x => x.Mlen(system.Bots[1].Position))
                        .Where(x => !system.Matrix.WillUnground(x));
                }

                var targetVoxel = targetVoxels.FirstOrDefault();

                // move bot to a nearby if we're not there
                if (!system.Bots[1].Position.Nearby.Contains(targetVoxel))
                {
                    // get a path to the nearest nearby
                    var pf = new AstarMatrixPather(system.Matrix);
                    List<Command> route = null;

                    foreach (var nb in targetVoxel.Nearby
                        .Where(x => !x.Filled)
                        .OrderByDescending(x => x.Y) // fill/void from the highest point
                        .ThenByDescending(x => x.Nearby.Count(y => y.Filled != y.Target && y.Grounded))
                        .ThenBy(x => x.Mlen(system.Bots[1].Position)))
                    {
                        var potentialRoute = pf.GetRouteTo(system.Bots[1].Position, nb);
                        if (potentialRoute.commands != null)
                            route = potentialRoute.commands;
                        break;
                    }

                    if (route == null)
                    {
                        TraceFile.WriteTraceFile(outputFile + ".failed", system.Trace);
                        Log.Error($"{Path.GetFileNameWithoutExtension(outputFile)} Failed with {targets.Count:N0} targets left");
                        return;
                    }

                    system.ExecuteCommands(CommandOptimizer.Compress(route));
                }

                // fill/void it
                if (!isDecompiling)
                    system.ExecuteCommand(1, new CommandFill(system.Bots[1].Position.Offset(targetVoxel)));
                else
                    system.ExecuteCommand(1, new CommandVoid(system.Bots[1].Position.Offset(targetVoxel)));

                // remove it from targets
                targets.Remove(targetVoxel);

                int complete = (int)(100 - ((float)targets.Count / startCount * 100));
                if (complete != lastComplete)
                {
                    lastComplete = complete;
                    Log.Info($"{Path.GetFileNameWithoutExtension(outputFile)} {complete:N0}% complete ({targets.Count:N0} remaining)");
                }
            }

            // move to home
            var pathFinder = new AstarMatrixPather(system.Matrix);
            var (_, homeRoute) = pathFinder.GetRouteTo(system.Bots[1].Position, system.Matrix.Get(0, 0, 0));

            if (homeRoute == null)
            {
                TraceFile.WriteTraceFile(outputFile + ".failed", system.Trace);
                Log.Error($"{Path.GetFileNameWithoutExtension(outputFile)} Failed with {targets.Count:N0} targets left");
                return;
            }

            system.ExecuteCommands(CommandOptimizer.Compress(homeRoute));

            // halt
            system.ExecuteCommand(1, new CommandHalt());

            TraceFile.WriteTraceFile(outputFile, system.Trace);

            Log.Debug($"{Path.GetFileNameWithoutExtension(outputFile)} Finished solution ");
        }
    }
}