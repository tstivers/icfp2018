using Contest.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Contest.Core.Octree
{
    public class VoxelOctree
    {
        protected readonly int _x1;
        protected readonly int _y1;
        protected readonly int _z1;
        protected readonly int _x2;
        protected readonly int _y2;
        protected readonly int _z2;

        public VoxelOctree(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            _x1 = x1;
            _y1 = y1;
            _z1 = z1;
            _x2 = x2;
            _y2 = y2;
            _z2 = z2;
        }

        protected const int MaxChildren = 100;

        protected VoxelOctree[] Children;

        protected List<Voxel> Nodes = new List<Voxel>();

        public void Add(Voxel voxel)
        {
            if (Children == null)
            {
                Nodes.Add(voxel);
                if (Nodes.Count > MaxChildren)
                    Split();
            }
            else
            {
                Children.First(x => x.Contains(voxel)).Add(voxel);
            }
        }

        public void Remove(Voxel voxel)
        {
            if (Children == null)
                Nodes.Remove(voxel);
            else
                Children.First(x => x.Contains(voxel)).Remove(voxel);
        }

        public IEnumerable<Voxel> GetNearby(Voxel target, int area)
        {
            return GetArea(target.X - area, target.Y - area, target.Z - area, target.X + area, target.Y + area,
                target.Z + area);
        }

        public IEnumerable<Voxel> GetArea(int ax, int ay, int az, int bx, int by, int bz)
        {
            if (Children != null)
            {
                return Children.Where(x => x.Contains(ax, ay, az, bx, by, bz))
                    .SelectMany(x => x.GetArea(ax, ay, az, bx, by, bz));
            }

            return Nodes.Where(x => x.X >= ax && x.X <= bx &&
                                    x.Y >= ay && x.Y <= by &&
                                    x.Z >= az && x.Z <= bz);
        }

        protected bool Contains(int ax, int ay, int az, int bx, int by, int bz)
        {
            return ax <= _x2 && bx >= _x1 &&
                   ay <= _y2 && by >= _y1 &&
                   az <= _z2 && bz >= _z1;
        }

        public int ChildCount
        {
            get { return 1 + Children?.Select(x => x.ChildCount).Sum() ?? 0; }
        }

        public int NodeCount
        {
            get { return Nodes?.Count ?? Children.Select(x => x.NodeCount).Sum(); }
        }

        protected void Split()
        {
            Children = new VoxelOctree[8];
            var cx = _x1 + (_x2 - _x1) / 2;
            var cy = _y1 + (_y2 - _y1) / 2;
            var cz = _z1 + (_z2 - _z1) / 2;

            Children[0] = new VoxelOctree(_x1, _y1, _z1, cx, cy, cz);
            Children[1] = new VoxelOctree(cx, _y1, _z1, _x2, cy, cz);
            Children[2] = new VoxelOctree(_x1, cy, _z1, cx, _y2, cz);
            Children[3] = new VoxelOctree(cx, cy, _z1, _x2, _y2, cz);

            Children[4] = new VoxelOctree(_x1, _y1, cz, cx, cy, _z2);
            Children[5] = new VoxelOctree(cx, _y1, cz, _x2, cy, _z2);
            Children[6] = new VoxelOctree(_x1, cy, cz, cx, _y2, _z2);
            Children[7] = new VoxelOctree(cx, cy, cz, _x2, _y2, _z2);

            foreach (var n in Nodes)
                Add(n);

            Nodes = null;
        }

        public override string ToString()
        {
            return $"<{_x1}, {_y1}, {_z1}> <{_x2}, {_y2}, {_z2}> [{(_x2 - _x1)}, {(_y2 - _y1)}, {(_z2 - _z1)}]";
        }

        protected bool Contains(Voxel voxel)
        {
            return voxel.X >= _x1 && voxel.X <= _x2 &&
                   voxel.Y >= _y1 && voxel.Y <= _y2 &&
                   voxel.Z >= _z1 && voxel.Z <= _z2;
        }
    }
}