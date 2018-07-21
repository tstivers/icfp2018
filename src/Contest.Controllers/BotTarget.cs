using Contest.Core.Models;
using System.Collections.Generic;

namespace Contest.Controllers
{
    public class BotTarget
    {
        public BotTarget(Coordinate c)
        {
            this.Coordinate = c;
            NearbyTargets = new List<Coordinate>();
        }

        public Coordinate Coordinate { get; }
        public List<Coordinate> NearbyTargets { get; set; }
        public int MLen { get; set; }
        public List<Command> Commands { get; set; }
    }
}