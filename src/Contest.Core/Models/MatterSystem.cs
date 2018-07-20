using Contest.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Contest.Core.Models
{
    public class MatterSystem
    {
        public bool Harmonics { get; set; }
        public Matrix Matrix { get; set; }
        public Dictionary<int, Bot> Bots { get; set; }
        public int Energy { get; set; }

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
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("wait", "bot does not exist");

            if (bot.Position != Coordinate.Zero)
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

        public void CmdSmove(int bid, CoordinateDifference d)
        {
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("smove", "bot does not exist");

            if (!d.IsLongLinear)
                throw new CommandException("smove", "not long linear difference");

            var c = bot.Position.Translate(d);

            var mlen = Matrix.CalcStraightMove(bot.Position, c);

            if (mlen <= 0)
                throw new CommandException("smove", "invalid move");

            bot.Position = c;
            Energy += 2 * mlen;
        }

        public void CmdLmove(int bid, CoordinateDifference d1, CoordinateDifference d2)
        {
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("lmove", "bot does not exist");

            if (!d1.IsShortLinear || !d2.IsShortLinear)
                throw new CommandException("smove", "not short linear difference");

            var b = bot.Position.Translate(d1);
            var c = bot.Position.Translate(d2);

            var mlen = Matrix.CalcLmove(bot.Position, b, c);

            if (mlen <= 0)
                throw new CommandException("lmove", "invalid move");

            bot.Position = c;
            Energy += 2 * (2 + mlen);
        }

        public void CmdFission(int bid, Coordinate c, int m)
        {
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("fission", "bot does not exist");

            throw new NotImplementedException();
        }

        public void CmdFill(int bid, CoordinateDifference d)
        {
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("fill", "bot does not exist");

            if (!d.IsNear)
                throw new CommandException("fill", "not near difference");

            var c = bot.Position.Translate(d);

            if (Matrix.Get(c.x, c.y, c.z))
            {
                Energy += 6;
            }
            else
            {
                Matrix.Set(c.x, c.y, c.z);
                Energy += 12;
            }
        }
    }
}