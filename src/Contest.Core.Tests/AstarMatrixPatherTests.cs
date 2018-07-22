using Contest.Controllers;
using Contest.Core.Helpers;
using Contest.Core.Models;
using NUnit.Framework;

namespace Contest.Core.Tests
{
    [TestFixture]
    internal class AstarMatrixPatherTests
    {
        [Test]
        public void TestPath()
        {
            var m = new Matrix(20, null, null);

            var pather = new AstarMatrixPather(m);

            var path = pather.GetRouteTo(m.Get(19, 19, 19), m.Get(0, 0, 0));

            Assert.That(path.commands.Count, Is.GreaterThan(0));

            var compressed = CommandOptimizer.Compress(path.commands);

            Assert.That(path.commands.Count, Is.GreaterThan(compressed.Count));
        }
    }
}