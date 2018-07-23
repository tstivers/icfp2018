using Contest.Core.Models;
using Priority_Queue;
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

            var goal = map.Get(start.X, 0, start.Z);
            var open_set = new SimplePriorityQueue<Voxel>();
            var closed_set = new HashSet<Voxel>();
            var g_score = new Dictionary<Voxel, float> { { start, 0 } };
            var f_score = new Dictionary<Voxel, float> { { start, heuristic_cost_estimate(start, goal) } };
            var came_from = new Dictionary<Voxel, Voxel>();

            open_set.Enqueue(start, float.MaxValue);

            while (open_set.Count > 0)
            {
                var current = open_set.Dequeue();
                goal = map.Get(current.X, 0, current.Z);

                if (current.Y == 0)
                {
                    if (sw.ElapsedMilliseconds > 1000)
                        Log.Debug($"Generated path to ground in {sw.ElapsedMilliseconds:N0} ms");

                    return true;
                }

                closed_set.Add(current);

                var adjacent = AdjacentFilledVoxels(current, avoid);

                foreach (var neighbor in adjacent)
                {
                    if (closed_set.Contains(neighbor))
                        continue;

                    var tentative_g_score = g_score[current] + (neighbor.Y < current.Y ? 0.5f : 1);

                    if (!open_set.Contains(neighbor))
                        open_set.Enqueue(neighbor, float.MaxValue);
                    else if (tentative_g_score >= g_score[neighbor])
                        continue;

                    g_score[neighbor] = tentative_g_score;
                    f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal);
                    open_set.UpdatePriority(neighbor, f_score[neighbor]);
                }
            }

            if (sw.ElapsedMilliseconds > 1000)
                Log.Debug($"Failed to generate path to ground in {sw.ElapsedMilliseconds:N0} ms");

            return false;
        }

        private static float heuristic_cost_estimate(Voxel current, Voxel goal)
        {
            return current.Mlen(goal);
        }

        public static IEnumerable<Voxel> AdjacentFilledVoxels(Voxel origin, Voxel avoid)
        {
            return origin.Adjacent.Where(x => x.Filled && x != avoid);
        }
    }
}