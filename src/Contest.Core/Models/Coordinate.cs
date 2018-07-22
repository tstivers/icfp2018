using System;
using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class Coordinate
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

        public static readonly Coordinate Zero = new Coordinate(0, 0, 0);

        public Coordinate Translate(CoordinateDifference d)
        {
            return new Coordinate((byte)(X + d.x), (byte)(Y + d.y), (byte)(Z + d.z));
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public int Mlen(Coordinate b)
        {
            return Math.Abs(X - b.X) + Math.Abs(Y - b.Y) + Math.Abs(Z - b.Z);
        }

        public CoordinateDifference GetDifference(Coordinate t)
        {
            return new CoordinateDifference((sbyte)(X - t.X), (sbyte)(Y - t.Y), (sbyte)(Z - t.Z));
        }

        public override bool Equals(object obj)
        {
            var coordinate = obj as Coordinate;
            return coordinate != null &&
                   X == coordinate.X &&
                   Y == coordinate.Y &&
                   Z == coordinate.Z;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Coordinate coordinate1, Coordinate coordinate2)
        {
            return EqualityComparer<Coordinate>.Default.Equals(coordinate1, coordinate2);
        }

        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return !(coordinate1 == coordinate2);
        }
    }
}