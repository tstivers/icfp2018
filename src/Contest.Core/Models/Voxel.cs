using System;
using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class Voxel : Coordinate
    {
        public bool Filled;
        public bool Target;
        public bool Volatile;
        public bool Grounded;

        public Voxel[] Adjacent;
        public Voxel[] Nearby;
        public List<Tuple<Voxel, IMoveCommand>> Neighbors;

        public Voxel(byte x, byte y, byte z) : base(x, y, z)
        {
        }
    }
}