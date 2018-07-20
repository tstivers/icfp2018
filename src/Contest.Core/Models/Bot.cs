using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class Bot
    {
        public int Bid { get; set; }
        public Coordinate Position { get; set; }
        public List<int> Seeds { get; set; }

        public Bot(int id, Coordinate position, List<int> seeds)
        {
            Bid = id;
            Position = position;
            Seeds = seeds ?? new List<int>();
        }

        public override string ToString()
        {
            return $"[{Bid}] {Position}";
        }
    }
}