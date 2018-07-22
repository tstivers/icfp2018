using Contest.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Contest.Core.Models
{
    public class MatterSystem
    {
        public bool Harmonics { get; set; }
        public Matrix Matrix { get; set; }
        public Dictionary<int, Bot> Bots { get; set; }
        public Trace Trace { get; set; }

        public MatterSystem(byte resolution, BitArray targetState, BitArray sourceState)
        {
            Harmonics = false; // low
            Matrix = new Matrix(resolution, targetState, sourceState);
            var startBot = new Bot(1, Matrix.Get(0, 0, 0), Enumerable.Range(2, 40).ToList());
            Bots = new Dictionary<int, Bot>();
            Bots.Add(startBot.Bid, startBot);
            Trace = new Trace();
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

            bot.Position = Matrix.Get(bot.Position.X + d.x, bot.Position.Y + d.y, bot.Position.Z + d.z);
        }

        public void CmdLmove(int bid, CoordinateDifference d1, CoordinateDifference d2)
        {
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("lmove", "bot does not exist");

            bot.Position = Matrix.Get(bot.Position.X + d1.x + d2.x, bot.Position.Y + d1.y + d2.y, bot.Position.Z + d1.z + d2.z);
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

            Matrix.Get(bot.Position.X + d.x, bot.Position.Y + d.y, bot.Position.Z + d.z).Filled = true;
        }

        public void CmdVoid(int bid, CoordinateDifference d)
        {
            if (!Bots.TryGetValue(bid, out var bot))
                throw new CommandException("void", "bot does not exist");

            Matrix.Get(bot.Position.X + d.x, bot.Position.Y + d.y, bot.Position.Z + d.z).Filled = false;
        }

        public void ExecuteCommand(int bid, Command c)
        {
            if (c is CommandHalt ch)
            {
                this.CmdHalt(bid);
            }

            if (c is CommandFill cf)
            {
                this.CmdFill(bid, cf.nd);
            }

            if (c is CommandVoid cv)
            {
                this.CmdVoid(bid, cv.nd);
            }

            if (c is CommandFlip flip)
            {
                this.CmdFlip(bid);
            }

            if (c is CommandWait)
            {
                this.CmdWait(bid);
            }

            if (c is CommandLmove lmove)
            {
                this.CmdLmove(bid, lmove.d1, lmove.d2);
            }

            if (c is CommandSmove smove)
            {
                this.CmdSmove(bid, smove.d);
            }

            Trace.Commands.Add(c);
        }

        public void ExecuteCommands(List<Command> targetCommands)
        {
            foreach (var c in targetCommands)
            {
                ExecuteCommand(1, c);
            }
        }
    }
}