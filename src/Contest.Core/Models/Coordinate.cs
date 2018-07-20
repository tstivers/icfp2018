using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class Coordinate
    {
        private readonly byte[] _p = new byte[3];

        public Coordinate(byte x, byte y, byte z)
        {
            _p[0] = x;
            _p[1] = y;
            _p[2] = z;
        }

        public Coordinate(Coordinate source)
        {
            _p = source._p;
        }

        public byte x => _p[0];

        public byte y => _p[1];

        public byte z => _p[2];

        public static readonly Coordinate Zero = new Coordinate(0, 0, 0);

        public Coordinate Translate(CoordinateDifference d)
        {
            return new Coordinate((byte)(x + d.x), (byte)(y + d.y), (byte)(z + d.z));
        }

        public override string ToString()
        {
            return $"<{x}, {y}, {z}>";
        }

        public override bool Equals(object obj)
        {
            var coordinate = obj as Coordinate;
            return coordinate != null &&
                   x == coordinate.x &&
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
            return EqualityComparer<Coordinate>.Default.Equals(coordinate1, coordinate2);
        }

        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return !(coordinate1 == coordinate2);
        }
    }
}