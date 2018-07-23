using Contest.Core.Models;
using System;

namespace Contest.Controllers
{
    public class Assembler : Solver
    {
        public Assembler(string targetFile, string sourceFile, string outputFile) : base(targetFile, sourceFile)
        {
        }

        public override void ToggleVoxel(Voxel targetVoxel)
        {
            MatterSystem.ExecuteCommand(1, new CommandFill(MatterSystem.Bots[1].Position.Offset(targetVoxel)));
        }

        public override (Voxel target, Voxel moveTarget) ChooseNextTarget()
        {
            throw new NotImplementedException();
        }
    }
}