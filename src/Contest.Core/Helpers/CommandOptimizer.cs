using Contest.Core.Models;
using System.Collections.Generic;

namespace Contest.Core.Helpers
{
    public static class CommandOptimizer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Command> Compress(List<Command> commands)
        {
            int count = 0;

            while (count != commands.Count)
            {
                count = commands.Count;
                commands = CompressSmoves(commands);
            }

            return commands;
        }

        private static List<Command> CompressSmoves(List<Command> commands)
        {
            var l = new List<Command>();
            var tmp = new List<Command>(commands);

            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i] is CommandSmove c1 && i + 1 < tmp.Count && tmp[i + 1] is CommandSmove c2 && c1.d.Direction == c2.d.Direction && c1.d.clen + c2.d.clen <= 15)
                {
                    var newCommand = new CommandSmove(new CoordinateDifference((sbyte)(c1.d.X + c2.d.X),
                        (sbyte)(c1.d.Y + c2.d.Y), (sbyte)(c1.d.Z + c2.d.Z)));
                    tmp[i + 1] = newCommand;
                    continue;
                }

                if (tmp[i] is CommandSmove l1 && i + 1 < tmp.Count && tmp[i + 1] is CommandSmove l2 && l1.d.clen <= 5 && l2.d.clen <= 5 && l1.d.Direction != l2.d.Direction)
                {
                    var newCommand = new CommandLmove(l1.d, l2.d);
                    tmp[i + 1] = newCommand;
                    continue;
                }

                if (tmp[i] is CommandLmove k1 && i + 1 < tmp.Count && tmp[i + 1] is CommandSmove k2 && k1.d2.Direction == k2.d.Direction && k1.d2.clen + k2.d.clen <= 5)
                {
                    var newCommand = new CommandLmove(k1.d1, new CoordinateDifference((sbyte)(k1.d2.X + k2.d.X), (sbyte)(k1.d2.Y + k2.d.Y), (sbyte)(k1.d2.Z + k2.d.Z)));
                    tmp[i + 1] = newCommand;
                    continue;
                }

                if (tmp[i] is CommandSmove q1 && i + 1 < tmp.Count && tmp[i + 1] is CommandLmove q2 && q1.d.Direction == q2.d1.Direction && q1.d.clen + q2.d1.clen <= 5)
                {
                    var newCommand = new CommandLmove(new CoordinateDifference((sbyte)(q1.d.X + q2.d1.X), (sbyte)(q1.d.Y + q2.d1.Y), (sbyte)(q1.d.Z + q2.d1.Z)), q2.d2);
                    tmp[i + 1] = newCommand;
                    continue;
                }

                l.Add(tmp[i]);
            }

            return l;
        }
    }
}