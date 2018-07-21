using System;
using System.Collections.Generic;

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

        public override string ToString()
        {
            return $"{{{x}, {y}, {z}}}";
        }

        public sbyte x => _p[0];

        public sbyte y => _p[1];

        public sbyte z => _p[2];

        public int mlen => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
        public int clen => Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));
        public int len => Math.Abs(x + y + z);

        public bool IsShortLinear => x == 0 && y == 0 && Math.Abs(z) <= 5 ||
                                     x == 0 && z == 0 && Math.Abs(y) <= 5 ||
                                     y == 0 && z == 0 && Math.Abs(x) <= 5;

        public bool IsLongLinear => x == 0 && y == 0 && Math.Abs(z) <= 15 ||
                                    x == 0 && z == 0 && Math.Abs(y) <= 15 ||
                                    y == 0 && z == 0 && Math.Abs(x) <= 15;

        public bool IsNear => mlen <= 2 && clen == 1;

        public byte Direction => x == 0 ? y == 0 ? (byte)0b11 : (byte)0b10 : (byte)0b01;

        public static readonly CoordinateDifference[] NearDifferences = GenerateNearDistances();

        private static CoordinateDifference[] GenerateNearDistances()
        {
            List<CoordinateDifference> nearDistances = new List<CoordinateDifference>();

            for (sbyte x = -1; x <= 1; x++)
                for (sbyte y = -1; y <= 1; y++)
                    for (sbyte z = -1; z <= 1; z++)
                    {
                        var d = new CoordinateDifference(x, y, z);
                        if (d.IsNear)
                            nearDistances.Add(d);
                    }

            return nearDistances.ToArray();
        }
    }
}