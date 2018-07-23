using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contest.Core;
using Contest.Core.Models;

namespace Contest.Controllers
{
    public class Reassembler
    {
        private string _sourceFile;
        private string _targetFile;

        public Reassembler(string targetFile, string sourceFile)
        {
            _targetFile = targetFile;
            _sourceFile = sourceFile;
        }

        public void Solve(string outputFile)
        {
            var d = new Deconstructor(_sourceFile);
            d.Solve(null);
            
            var a = new Assembler(_targetFile);
            a.MatterSystem.Trace = d.MatterSystem.Trace;
            a.Solve(outputFile);
        }
    }
}
