using System;
using System.Collections;
using System.Linq;

namespace Contest.Core.Models
{
    public class Matrix
    {
        public Matrix(byte resolution)
        {
            _r = resolution;
            Storage = new BitArray(Resolution ^ 3);
        }

        public Matrix(byte resolution, byte[] data)
        {
            _r = resolution;
            Storage = new BitArray(data);
        }

        private readonly byte _r;

        public byte Resolution => _r;

        public readonly BitArray Storage;

        public void Set(int x, int y, int z)
        {
            Storage.Set(x * _r * _r + y * _r + z, true);
        }

        public bool Get(int x, int y, int z)
        {
            return Storage.Get(x * _r * _r + y * _r + z);
        }

        public bool IsValidCoordinate(Coordinate c)
        {
            return c.x < _r && c.y < _r && c.z < _r;
        }

        public bool ValidCoordinates(params Coordinate[] values)
        {
            return values.All(IsValidCoordinate);
        }

        public int CalcStraightMove(Coordinate a, Coordinate b)
        {
            if (!ValidCoordinates(a, b))
                return -2;

            if (a.x == b.x && a.y == b.y) // z axis
            {
                for (int z = Math.Min(a.z, b.z); z <= Math.Max(a.z, b.z); z++)
                {
                    if (Get(a.x, a.y, z))
                        return -1;
                }

                return Math.Abs(a.z - b.z);
            }

            if (a.x == b.x && a.z == b.z) // y axis
            {
                for (int y = Math.Min(a.y, b.y); y <= Math.Max(a.y, b.y); y++)
                {
                    if (Get(a.x, y, a.z))
                        return -1;
                }

                return Math.Abs(a.y - b.y);
            }

            if (a.y == b.y && a.z == b.z) // x axis
            {
                for (int x = Math.Min(a.x, b.x); x <= Math.Max(a.x, b.x); x++)
                {
                    if (Get(x, a.y, a.z))
                        return -1;
                }

                return Math.Abs(a.y - b.y);
            }

            return 0; // not straight
        }

        public int CalcLmove(Coordinate a, Coordinate b, Coordinate c)
        {
            var mlena = CalcStraightMove(a, b);
            if (mlena <= 0)
                return mlena;

            var mlenb = CalcStraightMove(b, c);
            if (mlenb <= 0)
                return mlenb;

            return mlena + mlenb;
        }
    }
}