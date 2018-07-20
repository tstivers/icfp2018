using Contest.Core.Models;
using NUnit.Framework;
using System.IO;

namespace Contest.Core.Tests
{
    [TestFixture]
    public class SystemTests
    {
        [SetUp]
        public void SetUp()
        {
            Directory.SetCurrentDirectory(@"c:\users\tstivers\source\repos\icfp2018");
        }

        [Test]
        public void ExecuteTraceFile()
        {
            var target = MdlFile.LoadModel("problems/LA001_tgt.mdl");

            var system = new MatterSystem(target.Resolution);

            var trace = TraceFile.LoadTraceFile("traces/LA001.nbt");

            foreach (var c in trace.Commands)
            {
                if (c is CommandHalt ch)
                {
                    system.CmdHalt(0);
                }

                if (c is CommandFill cf)
                {
                    system.CmdFill(0, cf.nd);
                }

                if (c is CommandFlip flip)
                {
                    system.CmdFlip(0);
                }

                if (c is CommandWait)
                {
                    system.CmdWait(0);
                }

                if (c is CommandLmove lmove)
                {
                    system.CmdLmove(0, lmove.d1, lmove.d2);
                }

                if (c is CommandSmove smove)
                {
                    system.CmdSmove(0, smove.d);
                }
            }

            MdlFile.SaveModel("output/LA001.mdl", system.Matrix);
        }
    }
}