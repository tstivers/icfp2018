using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Contest.Core.Models
{
    public class Matrix
    {
        public Matrix(byte resolution)
        {
            _r = resolution;
            Storage = new BitArray((int)Math.Pow(_r, 3));
        }

        public Matrix(Matrix parent)
        {
            _r = parent.Resolution;
            Storage = new BitArray(parent.Storage);
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
            var index = x * _r * _r + y * _r + z;
            return Storage.Get(index);
        }

        public bool Get(Coordinate tc)
        {
            var index = tc.x * _r * _r + tc.y * _r + tc.z;
            return Storage.Get(index);
        }

        public bool IsValidCoordinate(Coordinate c)
        {
            return c.x < _r && c.y < _r && c.z < _r;
        }

        public bool ValidCoordinates(params Coordinate[] values)
        {
            return values.All(IsValidCoordinate);
        }

        public List<Coordinate> GetValidNearbies(Coordinate start)
        {
            var l = new List<Coordinate>();

            foreach (var d in CoordinateDifference.NearDifferences)
            {
                var c = start.Translate(d);
                if (IsValidCoordinate(c) && !Get(c.x, c.y, c.z))
                    l.Add(c);
            }

            return l;
        }

        public int CalcSmove(Coordinate a, Coordinate b)
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

                return Math.Abs(a.x - b.x);
            }

            return 0; // not straight
        }

        public int CalcLmove(Coordinate a, Coordinate b, Coordinate c)
        {
            var mlena = CalcSmove(a, b);
            if (mlena <= 0)
                return mlena;

            var mlenb = CalcSmove(b, c);
            if (mlenb <= 0)
                return mlenb;

            return mlena + mlenb;
        }

        public bool IsGrounded(Coordinate tc)
        {
            if (tc.y == 0)
                return true;

            if (tc.x > 0 && Get(tc.x - 1, tc.y, tc.z))
                return true;

            if (tc.x + 1 < Resolution && Get(tc.x + 1, tc.y, tc.z))
                return true;

            if (tc.y > 0 && Get(tc.x, tc.y - 1, tc.z))
                return true;

            if (tc.y + 1 < Resolution && Get(tc.x, tc.y + 1, tc.z))
                return true;

            if (tc.z > 0 && Get(tc.x, tc.y, tc.z - 1))
                return true;

            if (tc.z + 1 < Resolution && Get(tc.x, tc.y, tc.z + 1))
                return true;

            return false;
        }
    }
}