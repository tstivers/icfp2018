using System;

namespace Contest.Core.Models
{
    public class CoordinateDifference
    {
        private readonly sbyte[] _p = new sbyte[3];

        public CoordinateDifference(sbyte x, sbyte y, sbyte z)
        {
            _p[0] = x;
            _p[1] = y;
            _p[2] = z;
        }

        public sbyte x => _p[0];

        public sbyte y => _p[1];

        public sbyte z => _p[2];

        public int mlen => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
        public int clen => Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));

        public bool IsShortLinear => x == 0 && y == 0 && Math.Abs(z) <= 5 ||
                                     x == 0 && z == 0 && Math.Abs(y) <= 5 ||
                                     y == 0 && z == 0 && Math.Abs(x) <= 5;

        public bool IsLongLinear => x == 0 && y == 0 && Math.Abs(z) <= 15 ||
                                    x == 0 && z == 0 && Math.Abs(y) <= 15 ||
                                    y == 0 && z == 0 && Math.Abs(x) <= 15;

        public bool IsNear => mlen <= 2 && clen == 1;
    }
}