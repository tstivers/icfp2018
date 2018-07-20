using System;
using System.IO;
using System.Linq;

namespace Contest.Core.Models
{
    public class MdlFile
    {
        public static Matrix LoadModel(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException($"file {filename} does not exist", nameof(filename));

            var bytes = File.ReadAllBytes(filename);

            var matrix = new Matrix(bytes[0], bytes.Skip(1).ToArray());

            return matrix;
        }

        public static void SaveModel(string filename, Matrix matrix)
        {
            byte[] ret = new byte[(matrix.Storage.Length - 1) / 8 + 1];
            matrix.Storage.CopyTo(ret, 0);
            File.WriteAllBytes(filename, new[] { matrix.Resolution }.Concat(ret).ToArray());
        }
    }
}