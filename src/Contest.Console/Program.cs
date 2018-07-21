using Contest.Controllers;

namespace Contest.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var solver = new Solver();
            solver.Solve("../../../../problems/LA005_tgt.mdl");
        }
    }
}