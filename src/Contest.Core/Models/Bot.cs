using System.Collections.Generic;
using System.Linq;

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

        public Bot(Bot y)
        {
            Bid = y.Bid;
            Position = y.Position;
            Seeds = y.Seeds.ToList();
        }

        public override string ToString()
        {
            return $"[{Bid}] {Position}";
        }
    }
}