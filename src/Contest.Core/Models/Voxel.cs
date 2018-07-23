namespace Contest.Core.Models
{
    public class Voxel : Coordinate
    {
        private bool _filled;

        public bool Filled
        {
            get => _filled;
            set
            {
                _filled = value;

                if (_filled)
                {
                    foreach (var v in Adjacent)
                    {
                        v.Grounded = true;
                    }
                }
            }
        }

        public bool Target;
        public bool Volatile;
        public bool Grounded;

        public Voxel[] Adjacent;
        public Voxel[] Nearby;

        public Voxel(byte x, byte y, byte z) : base(x, y, z)
        {
            if (y == 0)
                Grounded = true;
        }
    }
}