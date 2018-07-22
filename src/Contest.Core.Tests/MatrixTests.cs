using Contest.Core.Models;
using NUnit.Framework;
using System.IO;

namespace Contest.Core.Tests
{
    [TestFixture]
    public class MatrixTests
    {
        [SetUp]
        public void SetUp()
        {
            Directory.SetCurrentDirectory(@"c:\users\tstivers\source\repos\icfp2018");
        }

        [Test]
        public void MatrixTest()
        {
            var target = MdlFile.LoadModel(@"problems/full/FA110_tgt.mdl");
        }
    }
}