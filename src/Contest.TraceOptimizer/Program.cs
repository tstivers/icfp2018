using Contest.Core;
using Contest.Core.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace Contest.TraceOptimizer
{
    public class Program
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

            Log.Debug("Compression complete");

            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        public static void OptimizeTrace(string inputfile, string outputfile)
        {
            var trace = TraceFile.LoadTraceFile(inputfile);
            var target = TraceFile.LoadTraceFile(outputfile);

            trace.Commands = CommandOptimizer.Compress(trace.Commands);

            if (target.Commands.Count != trace.Commands.Count)
                Log.Error($"{Path.GetFileName(inputfile)} : {target.Commands.Count} - {trace.Commands.Count}");
            else
                Log.Info($"{Path.GetFileName(inputfile)} : No Change");

            //TraceFile.WriteTraceFile(outputfile, trace);
        }
    }
}