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
                if (tmp[i] is CommandSmove c1 && i + 1 < tmp.Count && tmp[i + 1] is CommandSmove c2 && c1.d.Direction == c2.d.Direction && c1.d.len + c2.d.len <= 15)
                {
                    var newCommand = new CommandSmove(new CoordinateDifference((sbyte)(c1.d.x + c2.d.x),
                        (sbyte)(c1.d.y + c2.d.y), (sbyte)(c1.d.z + c2.d.z)));
                    tmp[i + 1] = newCommand;
                    continue;
                }

                if (tmp[i] is CommandSmove l1 && i + 1 < tmp.Count && tmp[i + 1] is CommandSmove l2 && l1.d.IsShortLinear && l2.d.IsShortLinear && l1.d.Direction != l2.d.Direction)
                {
                    var newCommand = new CommandLmove(l1.d, l2.d);
                    tmp[i + 1] = newCommand;
                    continue;
                }

                if (tmp[i] is CommandLmove k1 && i + 1 < tmp.Count && tmp[i + 1] is CommandSmove k2 && k1.d2.IsShortLinear && k2.d.IsShortLinear && k1.d2.Direction == k2.d.Direction && k1.d2.len + k2.d.len <= 5)
                {
                    var newCommand = new CommandLmove(k1.d1, new CoordinateDifference((sbyte)(k1.d2.x + k2.d.x), (sbyte)(k1.d2.y + k2.d.y), (sbyte)(k1.d2.z + k2.d.z)));
                    tmp[i + 1] = newCommand;
                    continue;
                }

                if (tmp[i] is CommandSmove q1 && i + 1 < tmp.Count && tmp[i + 1] is CommandLmove q2 && q1.d.Direction == q2.d1.Direction && q1.d.len + q2.d1.len <= 5)
                {
                    var newCommand = new CommandLmove(new CoordinateDifference((sbyte)(q1.d.x + q2.d1.x), (sbyte)(q1.d.y + q2.d1.y), (sbyte)(q1.d.z + q2.d1.z)), q2.d2);
                    tmp[i + 1] = newCommand;
                    continue;
                }

                l.Add(tmp[i]);
            }

            return l;
        }
    }
}