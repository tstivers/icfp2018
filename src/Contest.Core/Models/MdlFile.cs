using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Contest.Core.Models
{
    public class MdlFile
    {
        public static (byte, BitArray) LoadModel(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException($"file {Path.GetFullPath(filename)} does not exist", nameof(filename));

            var bytes = File.ReadAllBytes(filename);

            return (bytes[0], new BitArray(bytes.Skip(1).ToArray()));
        }

        public static void SaveModel(string filename, Matrix matrix)
        {
            byte[] ret = new byte[(matrix.Storage.Length - 1) / 8 + 1];
            matrix.GetBitArray().CopyTo(ret, 0);
            File.WriteAllBytes(filename, new[] { matrix.Resolution }.Concat(ret).ToArray());
        }
    }
}