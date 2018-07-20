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
    }
}