using Contest.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Contest.Core
{
    public class TraceFile
    {
        public static Trace LoadTraceFile(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException($"file {Path.GetFullPath(filename)} does not exist", nameof(filename));

            var bytes = File.ReadAllBytes(filename);

            var trace = new Trace();
            int ptr = 0;

            while (ptr < bytes.Length)
            {
                var result = ParseCommand(ptr, bytes);
                ptr += result.width;
                trace.Commands.Add(result.command);
            }

            return trace;
        }

        public static (int width, Command command) ParseCommand(int ptr, byte[] data)
        {
            if (data[ptr] == 0xff)
            {
                return (1, new CommandHalt());
            }

            if (data[ptr] == 0xfe)
            {
                return (1, new CommandWait());
            }

            if (data[ptr] == 0xfd)
            {
                return (1, new CommandFlip());
            }

            if ((data[ptr] & 0b11001111) == 0b00000100)
            {
                var a = (byte)(data[ptr] >> 4);
                var i = data[ptr + 1];
                return (2, new CommandSmove(ParseLongDifference(a, i)));
            }

            if ((data[ptr] & 0b00001111) == 0b00001100)
            {
                var sld1a = (data[ptr] >> 4) & 0b000000;
                var sld2a = data[ptr] >> 6;
                var sld1i = data[ptr + 1] & 0b00001111;
                var sld2i = data[ptr + 1] >> 4;
                return (2, new CommandLmove(ParseShortDifference(sld1a, sld1i), ParseShortDifference(sld2a, sld2i)));
            }

            if ((data[ptr] & 0b00000111) == 0b00000011)
            {
                var nd = data[ptr] >> 3;
                return (1, new CommandFill(ParseNearDifference(nd)));
            }

            throw new ArgumentException($"Unknown command {data[ptr]}");
        }

        public static CoordinateDifference ParseLongDifference(byte axis, byte length)

        {
            if (axis == 0b01)
            {
                return new CoordinateDifference((sbyte)(length - 15), 0, 0);
            }
            else if (axis == 0b10)
            {
                return new CoordinateDifference(0, (sbyte)(length - 15), 0);
            }
            else if (axis == 0b11)
            {
                return new CoordinateDifference(0, 0, (sbyte)(length - 15));
            }
            else
            {
                throw new ArgumentException($"Unknown axis {axis}");
            }
        }

        public static CoordinateDifference ParseShortDifference(int axis, int length)
        {
            if (axis == 0b01)
            {
                return new CoordinateDifference((sbyte)(length - 5), 0, 0);
            }
            else if (axis == 0b10)
            {
                return new CoordinateDifference(0, (sbyte)(length - 5), 0);
            }
            else if (axis == 0b11)
            {
                return new CoordinateDifference(0, 0, (sbyte)(length - 5));
            }
            else
            {
                throw new ArgumentException($"Unknown axis {axis}");
            }
        }

        public static CoordinateDifference ParseNearDifference(int code)
        {
            if (NearDifferenceLookup.TryGetValue(code, out var d))
                return d;

            throw new ArgumentException($"Unknown near distance {code}");
        }

        public static Dictionary<int, CoordinateDifference> NearDifferenceLookup = GenerateNearDistances();

        public static Dictionary<int, CoordinateDifference> GenerateNearDistances()
        {
            var lookup = new Dictionary<int, CoordinateDifference>();

            foreach (var d in CoordinateDifference.NearDifferences)
            {
                byte coded = (byte)((d.x + 1) * 9 + (d.y + 1) * 3 + (d.z + 1));

                lookup.Add(coded, d);
            }

            return lookup;
        }
    }
}