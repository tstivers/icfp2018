using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class Bot
    {
        public int Bid { get; set; }
        public Coordinate Position { get; set; }
        public List<int> Seeds { get; set; }
    }
}