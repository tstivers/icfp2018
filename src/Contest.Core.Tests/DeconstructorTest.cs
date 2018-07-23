using Contest.Controllers;
using Contest.Core.Models;
using NUnit.Framework;

namespace Contest.Core.Tests
{
    [TestFixture]
    internal class DeconstructorTest
    {
        [Test]
        public void Test()
        {
            var solver = new Deconstructor((byte)50);

            for (int x = 1; x < 50; x++)
                for (int y = 0; y < 10; y++)
                    for (int z = 1; z < 50; z++)
                        solver.MatterSystem.Matrix.Get(x, y, z).Filled = true;

            MdlFile.SaveModel(@"c:\users\tstivers\source\repos\icfp2018\problems\test\dst.mdl", solver.MatterSystem.Matrix);

            solver.Solve(@"c:\users\tstivers\source\repos\icfp2018\problems\test\dst.nbt");
        }
    }
}