using System.Collections;

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
    }
}