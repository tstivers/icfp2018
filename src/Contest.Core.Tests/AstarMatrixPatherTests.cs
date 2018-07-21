using Contest.Controllers;
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
            var m = new Matrix(20);

            var pather = new AstarMatrixPather(m);

            var path = pather.GetRouteTo(new Coordinate(0, 0, 0), new Coordinate(5, 2, 3));
        }
    }
}