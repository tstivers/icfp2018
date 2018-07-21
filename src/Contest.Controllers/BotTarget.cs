using Contest.Core.Models;
using System.Collections.Generic;

namespace Contest.Controllers
{
    public class BotTarget
    {
        public BotTarget(Coordinate c)
        {
            this.Coordinate = c;
            NearbyTargets = new SynchronizedCollection<Coordinate>();
        }

        public Coordinate Coordinate { get; }
        public IList<Coordinate> NearbyTargets { get; set; }
        public int MLen { get; set; }
        public List<Command> Commands { get; set; }
    }
}