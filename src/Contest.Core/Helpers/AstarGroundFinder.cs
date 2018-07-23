using Contest.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Priority_Queue;

namespace Contest.Core.Helpers
{
    public static class AstarGroundFinder
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool CanReachGround(Voxel start, Voxel avoid, Matrix map)
        {
            var sw = Stopwatch.StartNew();

            var goal = map.Get(map.Resolution / 2, 0, map.Resolution / 2);
            var closed_set = new HashSet<Voxel>();
            var g_score = new Dictionary<Voxel, float> { { start, 0 } };
            var f_score = new Dictionary<Voxel, float> { { start,  goal.Mlen(start)} };

            map.OpenSet.Clear();
            map.OpenSet.Enqueue(start, 0);

            while (map.OpenSet.Count > 0)
            {
                var current = map.OpenSet.Dequeue();

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

                    var tentative_g_score = g_score[current] + neighbor.Y < current.Y ? 0.5f : 1;

                    if (!map.OpenSet.Contains(neighbor) || tentative_g_score <= g_score[neighbor])
                    {
                        g_score[neighbor] = tentative_g_score;
                        f_score[neighbor] = g_score[neighbor] + goal.Mlen(neighbor);

                        if(map.OpenSet.Contains(neighbor))
                            map.OpenSet.UpdatePriority(neighbor, f_score[neighbor]);
                        else
                            map.OpenSet.Enqueue(neighbor, f_score[neighbor]);
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
    }
}