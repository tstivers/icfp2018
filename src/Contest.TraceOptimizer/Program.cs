using Contest.Core;
using Contest.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Contest.TraceOptimizer
{
    public class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            for (int i = 1; i <= 186; i++)
            {
                OptimizeTrace($"../../../../traces/full/FA{i:D3}.nbt", $"../../../../output/full/FA{i:D3}.nbt");
            }

            for (int i = 1; i <= 186; i++)
            {
                OptimizeTrace($"../../../../traces/full/FD{i:D3}.nbt", $"../../../../output/full/FD{i:D3}.nbt");
            }

            for (int i = 1; i <= 115; i++)
            {
                OptimizeTrace($"../../../../traces/full/FR{i:D3}.nbt", $"../../../../output/full/FR{i:D3}.nbt");
            }

            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        public static void OptimizeTrace(string inputfile, string outputfile)
        {
            var trace = TraceFile.LoadTraceFile(inputfile);

            int startCount = trace.Commands.Count;
            int count = 0;

            while (count != trace.Commands.Count)
            {
                count = trace.Commands.Count;
                trace.Commands = CompressSmoves(trace.Commands);
            }

            Log.Info($"Shrunk SMoves {Path.GetFileName(inputfile)} : {startCount} - {trace.Commands.Count}");

            startCount = trace.Commands.Count;
            count = 0;

            while (count != trace.Commands.Count)
            {
                count = trace.Commands.Count;
                trace.Commands = CompressLmoves(trace.Commands);
            }

            Log.Info($"Shrunk LMoves {Path.GetFileName(inputfile)} : {startCount} - {trace.Commands.Count}");

            TraceFile.WriteTraceFile(outputfile, trace);
        }

        private static List<Command> CompressSmoves(List<Command> commands)
        {
            var l = new List<Command>();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i] is CommandSmove c1 && i + 1 < commands.Count && commands[i + 1] is CommandSmove c2 && c1.d.Direction == c2.d.Direction && c1.d.len + c2.d.len <= 15)
                {
                    l.Add(new CommandSmove(new CoordinateDifference((sbyte)(c1.d.x + c2.d.x), (sbyte)(c1.d.y + c2.d.y), (sbyte)(c1.d.z + c2.d.z))));
                    i++;
                    continue;
                }

                l.Add(commands[i]);
            }

            return l;
        }

        private static List<Command> CompressLmoves(List<Command> commands)
        {
            if (commands.Count < 2)
                return commands;

            var l = new List<Command>();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i] is CommandSmove c1 && i + 1 < commands.Count && commands[i + 1] is CommandSmove c2)
                {
                    if (c1.d.IsShortLinear && c2.d.IsShortLinear && c1.d.Direction != c2.d.Direction)
                    {
                        l.Add(new CommandLmove(c1.d, c2.d));
                        i++;
                        continue;
                    }
                }

                l.Add(commands[i]);
            }

            return l;
        }
    }
}