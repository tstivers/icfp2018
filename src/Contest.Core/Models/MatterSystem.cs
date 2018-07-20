using System.Collections.Generic;

namespace Contest.Core.Models
{
    internal class MatterSystem
    {
        public int Energy { get; set; }
        public bool Harmonics { get; set; }
        public Matrix Matrix { get; set; }
        public Matrix TargetMatrix { get; set; }
        public List<Bot> Bots { get; set; }
        public Trace Trace { get; set; }
    }
}