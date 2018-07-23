using System.Linq;
using System.Threading.Tasks;
using Contest.Controllers;

namespace Contest.Console
{
    internal class Program
    {
        private static readonly int[] AssemblyProblems =
        {
            /*4, 8, 11, 17, 19, 20, 22, 32, 34, 44, 46, 52, 55, 61, 63, 80, 95, 97, 98,*/ /*99*//*, 108, 118, 140, 143, 156, 162,*/
            //164, 166, 169,
            //174
        };

        private static readonly int[] DeconstructionProblems =
        {
         /*   1, 14, 22, 35, 36, 44, 56, 76,*/ /*102,*/ /*103, 106,*/ /*107, 111,*//* 120,*/ /*124,*/ /*127,*//* 136,*//* 137,*/ /*151,*/ /*152,*//* 156,*//* 163,*/ /*167,*/
            /*168,*/ /*173, *//*175,*/ /*179,*/ 181
        };

        private static int[] ReassemblyProblems =
        {
           /* 5, 9, 14, 21, 24, 29, 47, 48, 58,*/ /*63, 67, *//*78, *//*83,*//* 85,*/ /*87,*/ /*100*/
        };

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            SolveReassembly();
        }

        public static void SolveAssembly()
        {
            var problems = Enumerable.Range(123, 185);

            Parallel.ForEach(problems, new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>
            {
                var solver = new Assembler($"../../../../problems/full/FA{i:D3}_tgt.mdl");
                solver.Solve($"../../../../output/full/FA{i:D3}.nbt");
            });
        }

        public static void SolveDeconstruction()
        {
            var problems = Enumerable.Range(1, 185).Skip(63);

            Parallel.ForEach(problems, new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>
            {
                var solver = new Deconstructor($"../../../../problems/full/FD{i:D3}_src.mdl");
                solver.Solve($"../../../../output/full/FD{i:D3}.nbt");
            });
        }

        public static void SolveReassembly()
        {
            var problems = Enumerable.Range(1, 114).Skip(68);

            Parallel.ForEach(problems, new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>
            {
                var solver = new Reassembler($"../../../../problems/full/FR{i:D3}_tgt.mdl", $"../../../../problems/full/FR{i:D3}_src.mdl");
                solver.Solve($"../../../../output/full/FR{i:D3}.nbt");
            });
        }
    }
}