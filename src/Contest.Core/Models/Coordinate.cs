using Priority_Queue;
using System;

namespace Contest.Core.Models
{
    public abstract class Coordinate
    {
        public Coordinate(byte x, byte y, byte z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public readonly byte X;

        public readonly byte Y;

        public readonly byte Z;

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public int Mlen(Coordinate b)
        {
            return Math.Abs(X - b.X) + Math.Abs(Y - b.Y) + Math.Abs(Z - b.Z);
        }

        public double dv(Coordinate b)
        {
            var dx = b.X - X;
            var dy = b.Y - Y;
            var dz = b.Z - Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public CoordinateDifference Offset(Coordinate targetVoxel)
        {
            return new CoordinateDifference((sbyte)(targetVoxel.X - this.X), (sbyte)(targetVoxel.Y - this.Y), (sbyte)(targetVoxel.Z - this.Z));
        }
    }
}