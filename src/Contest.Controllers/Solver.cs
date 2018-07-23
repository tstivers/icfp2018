using Contest.Core;
using Contest.Core.Helpers;
using Contest.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Contest.Controllers
{
    public abstract class Solver
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MatterSystem MatterSystem { get; set; }
        public string Name { get; set; }
        public HashSet<Voxel> RemainingVoxels { get; set; }

        public Solver(string targetFile, string sourceFile)
        {
            var targetData = (targetFile != null ? MdlFile.LoadModel(targetFile) : ((byte, BitArray)?)null);

            // load source
            var sourceData = (sourceFile != null ? MdlFile.LoadModel(sourceFile) : ((byte, BitArray)?)null);

            // create system
            MatterSystem = new MatterSystem(targetData?.Item1 ?? sourceData?.Item1 ?? 0, targetData?.Item2, sourceData?.Item2);
        }

        public Solver(byte resolution)
        {
            MatterSystem = new MatterSystem(resolution, null, null);
        }

        public void Solve(string outputFile)
        {
            if (outputFile != null)
                Name = Path.GetFileNameWithoutExtension(outputFile);

            // get all the target voxels
            RemainingVoxels = new HashSet<Voxel>(MatterSystem.Matrix.Storage.Where(x => x.Filled != x.Target));

            var lastCompletePercent = 0;
            var startCount = RemainingVoxels.Count;

            Log.Info($"{Name} Total targets: {RemainingVoxels.Count:N0}");

            while (RemainingVoxels.Count > 0)
            {
                var (targetVoxel, moveTarget) = ChooseNextTarget();

                // move bot to a nearby if we're not there
                if (MatterSystem.Bots[1].Position != moveTarget)
                {
                    var route = AstarMatrixPather.GetRouteTo(MatterSystem.Bots[1].Position, moveTarget);

                    MatterSystem.ExecuteCommands(CommandOptimizer.Compress(route));
                }

                // fill/void it
                ToggleVoxel(targetVoxel);

                // remove it from targets
                RemainingVoxels.Remove(targetVoxel);

                var complete = (int)(100 - ((float)RemainingVoxels.Count / startCount * 100));
                if (complete != lastCompletePercent)
                {
                    lastCompletePercent = complete;
                    Log.Info($"{Name} {complete:N0}% complete ({RemainingVoxels.Count:N0} remaining)");
                }
            }

            // move to home
            var homeRoute = AstarMatrixPather.GetRouteTo(MatterSystem.Bots[1].Position, MatterSystem.Matrix.Get(0, 0, 0));

            if (homeRoute == null)
            {
                TraceFile.WriteTraceFile(outputFile + ".failed", MatterSystem.Trace);
                Log.Error($"{Path.GetFileNameWithoutExtension(outputFile)} Failed with {RemainingVoxels.Count:N0} targets left");
                return;
            }

            MatterSystem.ExecuteCommands(CommandOptimizer.Compress(homeRoute));

            if (outputFile != null)
            {
                // halt
                MatterSystem.ExecuteCommand(1, new CommandHalt());

                // save trace file
                TraceFile.WriteTraceFile(outputFile, MatterSystem.Trace);
                Log.Debug($"{Path.GetFileNameWithoutExtension(outputFile)} Finished solution ");
            }
        }

        public abstract void ToggleVoxel(Voxel targetVoxel);

        public abstract (Voxel target, Voxel moveTarget) ChooseNextTarget();
    }
}