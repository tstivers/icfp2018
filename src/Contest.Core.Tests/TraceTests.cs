using NUnit.Framework;
using System.IO;

namespace Contest.Core.Tests
{
    [TestFixture]
    public class TraceTests
    {
        [SetUp]
        public void SetUp()
        {
            Directory.SetCurrentDirectory(@"c:\users\tstivers\source\repos\icfp2018");
        }

        [Test]
        public void LoadTraceFile()
        {
            var trace = TraceFile.LoadTraceFile("traces/LA001.nbt");
            Assert.That(trace.Commands.Count, Is.GreaterThan(0));
        }
    }
}