using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contest.Core.Helpers;
using Contest.Core.Models;
using NUnit.Framework;

namespace Contest.Core.Tests
{
    [TestFixture]
    public class GroundFinderTests
    {
        [Test]
        public void bleh()
        {
            var system = new MatterSystem(20, null, null);

            for(int x = 1; x < 20; x++)
            for (int y = 0; y < 20; y++)
            for (int z = 1; z < 20; z++)
            {
                system.Matrix.Get(x, y, z).Filled = true;
            }

            system.Matrix.Get(10, 0, 10).Filled = false;

            AstarGroundFinder.CanReachGround(system.Matrix.Get(10, 10, 10), null, system.Matrix);
        }

    }
}
