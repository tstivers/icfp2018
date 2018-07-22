using C5;
using Contest.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Contest.Core.Helpers
{
    public static class AstarGroundFinder
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool CanReachGround(Voxel start, Voxel avoid, Matrix map)
        {
            var sw = Stopwatch.StartNew();

            var closed_set = new System.Collections.Generic.HashSet<Voxel>();
            var g_score = new Dictionary<Voxel, float> { { start, 0 } };
            var f_score = new Dictionary<Voxel, float> { { start, start.Y } };
            var current = start;

            IPriorityQueue<Voxel> open_set = new IntervalHeap<Voxel>(new VoxelScoreComparer(f_score)) { start };
            var open_set_hash = new System.Collections.Generic.HashSet<Coordinate>();

            while (!open_set.IsEmpty)
            {
                current = open_set.DeleteMin();

                if (current.Y == 0)
                {
                    if (sw.ElapsedMilliseconds > 1000)
                        Log.Debug($"Generated path to ground in {sw.ElapsedMilliseconds:N0} ms");
                    return true;
                }

                closed_set.Add(current);

                foreach (var neighbor in AdjacentFilledVoxels(current, avoid))
                {
                    if (closed_set.Contains(neighbor))
                        continue;

                    var tentative_g_score = g_score[current] + 1;

                    if (!open_set_hash.Contains(neighbor) || tentative_g_score < g_score[neighbor])
                    {
                        g_score[neighbor] = tentative_g_score;
                        f_score[neighbor] = g_score[neighbor] + neighbor.Y;
                        open_set.Add(neighbor);
                        open_set_hash.Add(neighbor);
                    }
                }
            }

            if (sw.ElapsedMilliseconds > 1000)
                Log.Debug($"Failed to generate path to ground in {sw.ElapsedMilliseconds:N0} ms");

            return false;
        }

        public static IEnumerable<Voxel> AdjacentFilledVoxels(Voxel origin, Voxel avoid)
        {
            return origin.Adjacent.Where(x => x.Filled && x != avoid);
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