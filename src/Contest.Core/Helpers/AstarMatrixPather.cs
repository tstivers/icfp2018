using C5;
using Contest.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Contest.Controllers
{
    public class AstarMatrixPather
    {
        private readonly Matrix _matrix;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly System.Collections.Generic.HashSet<Voxel> _unreachable =
            new System.Collections.Generic.HashSet<Voxel>();

        public AstarMatrixPather(Matrix matrix)
        {
            _matrix = matrix;
        }

        public (int cost, List<Command> commands) GetRouteTo(Voxel start, Voxel goal)
        {
            var sw = Stopwatch.StartNew();

            var closed_set = new System.Collections.Generic.HashSet<Voxel>();
            var came_from = new Dictionary<Voxel, Voxel>();
            var g_score = new Dictionary<Voxel, float> { { start, 0 } };
            var f_score = new Dictionary<Voxel, float> { { start, GetGScore(start, goal) } };
            var current = start;

            IPriorityQueue<Voxel> open_set = new IntervalHeap<Voxel>(new VoxelScoreComparer(f_score)) { start };
            var open_set_hash = new System.Collections.Generic.HashSet<Coordinate>();

            if (_unreachable.Contains(goal))
                return (-1, null);

            while (!open_set.IsEmpty)
            {
                current = open_set.DeleteMin();

                if (current == goal)
                {
                    if (sw.ElapsedMilliseconds > 1000)
                        Log.Debug($"Generated path in {sw.ElapsedMilliseconds:N0} ms ({closed_set.Count:N0} closed, {came_from.Count:N0} examined)");
                    return ReconstructPath(came_from, start, goal);
                }

                closed_set.Add(current);
                came_from.TryGetValue(current, out var prev);

                var offset = prev == null ? new[] { 0, 0, 0 } : new[] { current.X - prev.X, current.Y - prev.Y, current.Z - prev.Z };

                foreach (var neighbor in AdjacentVoxels(current))
                {
                    if (closed_set.Contains(neighbor))
                        continue;

                    var tentative_g_score = g_score[current] + GetFScore(current, neighbor, offset);

                    if (!open_set_hash.Contains(neighbor) || tentative_g_score < g_score[neighbor])
                    {
                        g_score[neighbor] = tentative_g_score;
                        f_score[neighbor] = g_score[neighbor] + GetGScore(neighbor, goal);
                        open_set.Add(neighbor);
                        open_set_hash.Add(neighbor);
                        came_from[neighbor] = current;
                    }
                }
            }

            // fill out our unreachable points
            for (byte x = 0; x < _matrix.Resolution; x++)
            {
                for (byte y = 0; y < _matrix.Resolution; y++)
                {
                    for (byte z = 0; z < _matrix.Resolution; z++)
                    {
                        var v = _matrix.Get(x, y, z);
                        if (!closed_set.Contains(v))
                            _unreachable.Add(v);
                    }
                }
            }

            if (sw.ElapsedMilliseconds > 1000)
                Log.Debug($"Failed to generate path path in {sw.ElapsedMilliseconds:N0} ms ({closed_set.Count:N0} closed, {came_from.Count:N0} examined)");

            return (0, null);
        }

        private float GetFScore(Voxel current, Voxel neighbor, int[] offset)
        {
            if (current.X + offset[0] == neighbor.X &&
                current.Y + offset[1] == neighbor.Y &&
                current.Z + offset[2] == neighbor.Z)
                return 0.5f;

            return 1;
        }

        private (int cost, List<Command> commands) ReconstructPath(Dictionary<Voxel, Voxel> came_from, Voxel start, Voxel current)
        {
            List<Command> commands = new List<Command>();

            while (current != start)
            {
                var prev = came_from[current];
                commands.Add(new CommandSmove(new CoordinateDifference((sbyte)(current.X - prev.X), (sbyte)(current.Y - prev.Y), (sbyte)(current.Z - prev.Z))));

                current = prev;
            }

            commands.Reverse();

            return (commands.Count, commands);
        }

        public Voxel[] AdjacentVoxels(Voxel origin)
        {
            return origin.Adjacent;
        }

        private float GetHScore(Coordinate current, Coordinate neighbor)
        {
            return 1;
        }

        private float GetGScore(Coordinate start, Coordinate goal)
        {
            return start.Mlen(goal);
        }

        private class VoxelScoreComparer : IComparer<Voxel>
        {
            private readonly Dictionary<Voxel, float> _f_scores;

            public VoxelScoreComparer(Dictionary<Voxel, float> f_scores)
            {
                _f_scores = f_scores;
            }

            public int Compare(Voxel x, Voxel y)
            {
                return _f_scores[x].CompareTo(_f_scores[y]);
            }
        }
    }
}