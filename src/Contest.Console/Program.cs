using Contest.Controllers;

namespace Contest.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var solver = new Solver();

            for (int i = 1; i < 42; i++)
            {
                solver.Solve($"../../../../problems/LA{i:D3}_tgt.mdl", $"../../../../output/LA{i:D3}.nbt");
            }
        }
    }
}