using Contest.Core.Models;
using Contest.Core.Octree;
using System.Linq;
using C5;

namespace Contest.Controllers
{
    public class Deconstructor : Solver
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Deconstructor(string sourceFile) : base(null, sourceFile)
        {
        }

        public Deconstructor(byte resolution) : base(resolution)
        {
        }

        public VoxelOctree Octree;

        public override void ToggleVoxel(Voxel targetVoxel)
        {
            MatterSystem.ExecuteCommand(1, new CommandVoid(MatterSystem.Bots[1].Position.Offset(targetVoxel)));
            Octree.Remove(targetVoxel);
        }

        public override (Voxel target, Voxel moveTarget) ChooseNextTarget()
        {
            if (Octree == null)
            {
                Octree = new VoxelOctree(0, 0, 0, MatterSystem.Matrix.Resolution, MatterSystem.Matrix.Resolution, MatterSystem.Matrix.Resolution);
                foreach (var v in RemainingVoxels)
                    Octree.Add(v);
            }

            var pos = MatterSystem.Bots[1].Position;
            var pathFinder = new AstarMatrixPather(MatterSystem.Matrix);
            var ungrounded = new HashSet<Voxel>();

            // check nearbies
            foreach (var v in pos.Nearby.Where(x => x.Filled))
            {
                if (MatterSystem.Matrix.WillUnground(v))
                {
                    ungrounded.Add(v);
                    continue;
                }

                return (v, pos);
            }

            for (var i = 4; i < MatterSystem.Matrix.Resolution; i++)
            {
                var target = Octree.GetNearby(pos, i)
                    .Where(x => !ungrounded.Contains(x))
                    .OrderByDescending(x => x.Y)
                    .ThenBy(x => x.dv(pos));

                foreach (var v in target)
                {
                    if (MatterSystem.Matrix.WillUnground(v))
                    {
                        ungrounded.Add(v);
                        continue;
                    }

                    // see if we can navigate to this
                    var moveTargets = v.Nearby
                        .Where(x => !x.Filled)
                        .OrderByDescending(x => x.Y)
                        .ThenByDescending(x => x.Nearby.Count(y => y.Filled))
                        .ThenBy(x => x.dv(pos));

                    foreach (var mt in moveTargets)
                    {
                        if (pathFinder.GetRouteTo(pos, mt) == null)
                            continue;

                        return (v, mt);
                    }
                }
            }

            return (null, null);
        }
    }
}