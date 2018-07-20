using Contest.Core.Models;

namespace Contest.Controllers
{
    public class Solver
    {
        public MatterSystem _system;

        public void Solve(string filename)
        {
            var target = MdlFile.LoadModel(filename);

            _system = new MatterSystem(target.Resolution);

            while (_system.Matrix != target)
            {
                // move the bot to the first target
                // fill it
                // end
            }
        }
    }
}