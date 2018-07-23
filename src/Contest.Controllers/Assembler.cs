using Contest.Core.Models;
using Contest.Core.Octree;
using System.Linq;

namespace Contest.Controllers
{
    public class Assembler : Solver
    {
        public Assembler(string targetFile) : base(targetFile, null)
        {
        }

        public Assembler(byte resolution) : base(resolution)
        {
        }

        public VoxelOctree Octree;

        public override void ToggleVoxel(Voxel targetVoxel)
        {
            MatterSystem.ExecuteCommand(1, new CommandFill(MatterSystem.Bots[1].Position.Offset(targetVoxel)));
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

            for (var i = 4; i < MatterSystem.Matrix.Resolution; i++)
            {
                var target = Octree.GetNearby(pos, i)
                    .Where(x => x.Grounded)
                    .OrderBy(x => x.Y)
                    .ThenBy(x => x.dv(pos));


                foreach (var v in target)
                {
                    // see if we can navigate to this
                    var moveTargets = v.Nearby
                        .Where(x => !x.Filled)
                        .OrderByDescending(x => x.Y)
                        .ThenByDescending(x => x.Nearby.Count(y => y.Filled != y.Target && y.Grounded))
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