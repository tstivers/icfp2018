using Contest.Core.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Contest.Core.Models
{
    public class Matrix
    {
        private static readonly IMoveCommand[] MoveCommands = GenerateMoveCommands().ToArray();

        protected Matrix(byte resolution)
        {
            Resolution = resolution;
            Magnitude = (int)Math.Pow(Resolution, 3);
            Res2 = (int)Math.Pow(Resolution, 2);
            Storage = new Voxel[Magnitude];

            for (byte x = 0; x < Resolution; x++)
                for (byte y = 0; y < Resolution; y++)
                    for (byte z = 0; z < Resolution; z++)
                        Storage[x * Res2 + y * Resolution + z] = new Voxel(x, y, z);

            CalculateRelationships();
        }

        public Matrix(byte resolution, BitArray targetState, BitArray sourceState) : this(resolution)
        {
            for (int i = 0; i < Magnitude; i++)
            {
                if (targetState != null)
                    Storage[i].Target = targetState[i];

                if (sourceState != null)
                    Storage[i].Filled = sourceState[i];
            }
        }

        public BitArray GetBitArray()
        {
            var array = new BitArray(Magnitude);
            for (int i = 0; i < Magnitude; i++)
                array[i] = Storage[i].Filled;
            return array;
        }

        public readonly byte Resolution;
        public readonly int Res2;
        public int Magnitude;
        public readonly Voxel[] Storage;

        public Voxel Get(int x, int y, int z)
        {
            return Storage[x * Res2 + y * Resolution + z];
        }

        public void CalculateRelationships()
        {
            for (byte x = 0; x < Resolution; x++)
                for (byte y = 0; y < Resolution; y++)
                    for (byte z = 0; z < Resolution; z++)
                    {
                        var voxel = Get(x, y, z);
                        CalculateAdjacent(voxel);
                        CalculateNearby(voxel);
                    }
        }

        public void CalculateAdjacent(Voxel voxel)
        {
            var adjacent = new List<Voxel>();

            if (voxel.X > 0)
                adjacent.Add(Get(voxel.X - 1, voxel.Y, voxel.Z));

            if (voxel.X < Resolution - 1)
                adjacent.Add(Get(voxel.X + 1, voxel.Y, voxel.Z));

            if (voxel.Y > 0)
                adjacent.Add(Get(voxel.X, voxel.Y - 1, voxel.Z));

            if (voxel.Y < Resolution - 1)
                adjacent.Add(Get(voxel.X, voxel.Y + 1, voxel.Z));

            if (voxel.Z > 0)
                adjacent.Add(Get(voxel.X, voxel.Y, voxel.Z - 1));

            if (voxel.Z < Resolution - 1)
                adjacent.Add(Get(voxel.X, voxel.Y, voxel.Z + 1));

            voxel.Adjacent = adjacent.ToArray();
        }

        public bool ValidCoord(int x, int y, int z)
        {
            return x > 0 && x < Resolution && y > 0 && y < Resolution && z > 0 && z < Resolution;
        }

        public void CalculateNearby(Voxel voxel)
        {
            var nearby = new List<Voxel>();

            for (sbyte x = -1; x <= 1; x++)
                for (sbyte y = -1; y <= 1; y++)
                    for (sbyte z = -1; z <= 1; z++)
                    {
                        var d = new CoordinateDifference(x, y, z);
                        if (d.IsNear)
                        {
                            var c = voxel.Translate(d);
                            if (ValidCoord(c.X, c.Y, c.Z))
                                nearby.Add(Get(c.X, c.Y, c.Z));
                        }
                    }

            voxel.Nearby = nearby.ToArray();
        }

        public void RefreshAllNodes()
        {
            for (byte x = 0; x < Resolution; x++)
                for (byte y = 0; y < Resolution; y++)
                    for (byte z = 0; z < Resolution; z++)
                    {
                        var voxel = Get(x, y, z);
                        RefreshNode(voxel);
                    }
        }

        private static IMoveCommand[] GenerateMoveCommands()
        {
            var list = new List<IMoveCommand>();

            // generate all possible smoves
            for (sbyte i = -15; i <= 15; i++)
            {
                if (i == 0)
                    continue;

                list.Add(new CommandSmove(new CoordinateDifference(i, 0, 0)));
                list.Add(new CommandSmove(new CoordinateDifference(0, i, 0)));
                list.Add(new CommandSmove(new CoordinateDifference(0, 0, i)));
            }

            // generate all possible lmoves
            for (sbyte i = -5; i <= 5; i++)
                for (sbyte j = -5; j <= 5; j++)
                {
                    if (i == 0 || j == 0)
                        continue;

                    list.Add(new CommandLmove(new CoordinateDifference(i, 0, 0), new CoordinateDifference(j, 0, 0)));
                    list.Add(new CommandLmove(new CoordinateDifference(0, i, 0), new CoordinateDifference(j, 0, 0)));
                    list.Add(new CommandLmove(new CoordinateDifference(0, 0, i), new CoordinateDifference(j, 0, 0)));

                    list.Add(new CommandLmove(new CoordinateDifference(i, 0, 0), new CoordinateDifference(0, j, 0)));
                    list.Add(new CommandLmove(new CoordinateDifference(0, i, 0), new CoordinateDifference(0, j, 0)));
                    list.Add(new CommandLmove(new CoordinateDifference(0, 0, i), new CoordinateDifference(0, j, 0)));

                    list.Add(new CommandLmove(new CoordinateDifference(i, 0, 0), new CoordinateDifference(0, 0, j)));
                    list.Add(new CommandLmove(new CoordinateDifference(0, i, 0), new CoordinateDifference(0, 0, j)));
                    list.Add(new CommandLmove(new CoordinateDifference(0, 0, i), new CoordinateDifference(0, 0, j)));
                }

            return list.ToArray();
        }

        public void RefreshNode(Voxel voxel)
        {
            if (voxel.Neighbors == null)
            {
                voxel.Neighbors = new List<Tuple<Voxel, IMoveCommand>>();
                foreach (var cmd in MoveCommands)
                {
                    var c = cmd.Destination(voxel);
                    if (!ValidCoord(c.X, c.Y, c.Z))
                        continue;

                    voxel.Neighbors.Add(Tuple.Create(Get(c.X, c.Y, c.Z), cmd));
                }
            }

            if (voxel.Neighbors.Count == 0)
                return;

            if (voxel.Filled)
            {
                voxel.Neighbors.Clear();
                return;
            }

            if (voxel.Adjacent.All(x => x.Filled))
            {
                voxel.Neighbors.Clear();
                return;
            }

            var validMoves = new List<Tuple<Voxel, IMoveCommand>>();

            // validate remaining commands
            foreach (var kvp in voxel.Neighbors)
            {
                if (kvp.Item1.Filled)
                    continue;

                // validate move
                if (!IsValidMove(voxel, kvp.Item2))
                    continue;

                validMoves.Add(kvp);
            }

            voxel.Neighbors = validMoves;
        }

        public bool IsValidMove(Coordinate start, IMoveCommand command)
        {
            return true;
        }

        public bool WillUnground(Voxel voxel)
        {
            foreach (var v in voxel.Adjacent.Where(x => x.Filled))
            {
                // find path from v to ground
                if (!AstarGroundFinder.CanReachGround(v, voxel, this))
                    return true;
            }

            return false;
        }
    }
}