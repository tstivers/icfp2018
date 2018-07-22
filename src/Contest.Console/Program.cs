using Contest.Controllers;
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

            var problems = Enumerable.Range(51, 100);

            Parallel.ForEach(problems, new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
              {
                  solver.Solve(null, $"../../../../problems/full/FD{i:D3}_src.mdl", $"../../../../output/full/FD{i:D3}.nbt");
              });
        }
    }
}