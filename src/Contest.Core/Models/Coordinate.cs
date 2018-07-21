using System;

namespace Contest.Core.Models
{
    public struct Coordinate
    {
        public Coordinate(byte x, byte y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public byte x;

        public byte y;

        public byte z;

        public static readonly Coordinate Zero = new Coordinate(0, 0, 0);

        public Coordinate Translate(CoordinateDifference d)
        {
            return new Coordinate((byte)(x + d.x), (byte)(y + d.y), (byte)(z + d.z));
        }

        public override string ToString()
        {
            return $"<{x}, {y}, {z}>";
        }

        public int Mlen(Coordinate b)
        {
            return Math.Abs(x - b.x) + Math.Abs(y - b.y) + Math.Abs(z - b.z);
        }

        public CoordinateDifference GetDifference(Coordinate t)
        {
            return new CoordinateDifference((sbyte)(x - t.x), (sbyte)(y - t.y), (sbyte)(z - t.z));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coordinate))
            {
                return false;
            }

            var coordinate = (Coordinate)obj;
            return x == coordinate.x &&
                   y == coordinate.y &&
                   z == coordinate.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Coordinate coordinate1, Coordinate coordinate2)
        {
            return coordinate1.Equals(coordinate2);
        }

        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return !(coordinate1 == coordinate2);
        }
    }
}