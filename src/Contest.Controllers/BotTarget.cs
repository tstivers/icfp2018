using Contest.Core.Models;
using System.Collections.Generic;

namespace Contest.Controllers
{
    public class BotTarget
    {
        public BotTarget(Coordinate c)
        {
            Coordinate = c;
            NearbyTargets = new List<Coordinate>();
        }

        public Coordinate Coordinate { get; }
        public IList<Coordinate> NearbyTargets { get; set; }
        public List<Command> Commands { get; set; }
    }
}