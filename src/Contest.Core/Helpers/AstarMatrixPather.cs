using Contest.Core.Models;
using Priority_Queue;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Contest.Controllers
{
    public static class AstarMatrixPather
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Command> GetRouteTo(Voxel start, Voxel goal)
        {
            var sw = Stopwatch.StartNew();

            var open_set = new SimplePriorityQueue<Voxel>();
            var closed_set = new HashSet<Voxel>();
            var g_score = new Dictionary<Voxel, float> { { start, 0 } };
            var f_score = new Dictionary<Voxel, float> { { start, heuristic_cost_estimate(start, goal) } };
            var came_from = new Dictionary<Voxel, Voxel>();

            open_set.Enqueue(start, float.MaxValue);

            while (open_set.Count > 0)
            {
                var current = open_set.Dequeue();

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

                    var tentative_g_score = g_score[current] + dist_between(current, neighbor, offset);

                    if (!open_set.Contains(neighbor))
                        open_set.Enqueue(neighbor, float.MaxValue);
                    else if (tentative_g_score >= g_score[neighbor])
                        continue;

                    came_from[neighbor] = current;
                    g_score[neighbor] = tentative_g_score;
                    f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal);
                    open_set.UpdatePriority(neighbor, f_score[neighbor]);
                }
            }

            if (sw.ElapsedMilliseconds > 1000)
                Log.Debug($"Failed to generate path path in {sw.ElapsedMilliseconds:N0} ms ({closed_set.Count:N0} closed, {came_from.Count:N0} examined)");

            return null;
        }

        private static float dist_between(Voxel current, Voxel neighbor, int[] offset)
        {
            if (current.X + offset[0] == neighbor.X &&
                current.Y + offset[1] == neighbor.Y &&
                current.Z + offset[2] == neighbor.Z)
                return 0.5f;

            return 1;
        }

        private static float heuristic_cost_estimate(Voxel neighbor, Voxel goal)
        {
            return neighbor.Mlen(goal);
        }

        public static IEnumerable<Voxel> AdjacentVoxels(Voxel origin)
        {
            return origin.Adjacent.Where(x => !x.Filled && !x.Volatile);
        }

        private static List<Command> ReconstructPath(Dictionary<Voxel, Voxel> came_from, Voxel start, Voxel current)
        {
            List<Command> commands = new List<Command>();

            while (current != start)
            {
                var prev = came_from[current];
                commands.Add(new CommandSmove(new CoordinateDifference((sbyte)(current.X - prev.X), (sbyte)(current.Y - prev.Y), (sbyte)(current.Z - prev.Z))));

                current = prev;
            }

            commands.Reverse();

            return commands;
        }
    }
}