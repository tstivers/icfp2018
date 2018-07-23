using System;
using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class CoordinateDifference
    {
        public CoordinateDifference(sbyte x, sbyte y, sbyte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{{{X}, {Y}, {Z}}}";
        }

        public sbyte X;
        public sbyte Y;
        public sbyte Z;

        public int mlen => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        public int clen => Math.Max(Math.Max(Math.Abs(X), Math.Abs(Y)), Math.Abs(Z));
        public bool IsNear => mlen <= 2 && clen == 1;

        public byte Direction => X == 0 ? Y == 0 ? (byte)0b11 : (byte)0b10 : (byte)0b01;

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