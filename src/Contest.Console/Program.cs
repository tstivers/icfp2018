using Contest.Controllers;
using Contest.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Contest.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var solver = new Solver();

            var maps = new Dictionary<int, int>();

            for (int i = 1; i < 187; i++)
            {
                var solvedDate = File.GetLastWriteTime($"../../../../output/LA{i:D3}.nbt");

                if (!(solvedDate < DateTime.Parse("7/21/2018")))
                    continue;

                var matrix = MdlFile.LoadModel($"../../../../problems/LA{i:D3}_tgt.mdl");
                maps.Add(i, matrix.Voxels);
            }

            var problems = maps.OrderBy(x => x.Value).Select(x => x.Key);

            Parallel.ForEach(problems, new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
              {
                  solver.Solve($"../../../../problems/LA{i:D3}_tgt.mdl", $"../../../../output/LA{i:D3}.nbt");
              });
        }
    }
}