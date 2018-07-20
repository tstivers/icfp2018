using Contest.Core.Exceptions;
using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class MatterSystem
    {
        public bool Harmonics { get; set; }
        public Matrix Matrix { get; set; }
        public Dictionary<int, Bot> Bots { get; set; }

        public MatterSystem(byte resolution)
        {
            Harmonics = false; // low
            Matrix = new Matrix(resolution);
            var startBot = new Bot(0, Coordinate.Zero, null);
            Bots = new Dictionary<int, Bot>();
            Bots.Add(startBot.Bid, startBot);
        }

        public void CmdHalt(int bid)
        {
            if (!Bots.ContainsKey(bid))
                throw new CommandException("halt", "bot does not exist");

            if (Bots[bid].Position != Coordinate.Zero)
                throw new CommandException("halt", "bot not at 0,0,0");

            if (Bots.Count > 1)
                throw new CommandException("halt", "more than one bot active");

            if (Harmonics)
                throw new CommandException("halt", "tried to halt with high harmonics");

            Bots.Remove(bid);
        }

        public void CmdWait(int bid)
        {
            if (!Bots.ContainsKey(bid))
                throw new CommandException("wait", "bot does not exist");
        }

        public void CmdFlip(int bid)
        {
            if (!Bots.ContainsKey(bid))
                throw new CommandException("flip", "bot does not exist");

            Harmonics = !Harmonics;
        }

        public void CmdSmove(int bid, Coordinate c)
        {
            if (!Bots.ContainsKey(bid))
                throw new CommandException("smove", "bot does not exist");
        }
    }
}