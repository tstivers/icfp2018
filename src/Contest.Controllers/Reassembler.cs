using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contest.Core.Models;

namespace Contest.Controllers
{
    public class Reassembler
    {
        public Reassembler(string targetFile, string sourceFile)
        {
            var targetData = (targetFile != null ? MdlFile.LoadModel(targetFile) : ((byte, BitArray)?)null);

            // load source
            var sourceData = (sourceFile != null ? MdlFile.LoadModel(sourceFile) : ((byte, BitArray)?)null);
        }

        public void Solve(string outputFile)
        {

        }
    }
}
