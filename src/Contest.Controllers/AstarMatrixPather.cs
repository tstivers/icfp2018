using C5;
using Contest.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Contest.Controllers
{
    public class AstarMatrixPather
    {
        private Matrix _map;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly System.Collections.Generic.HashSet<Coordinate> _unreachable =
            new System.Collections.Generic.HashSet<Coordinate>();

        public AstarMatrixPather(Matrix map)
        {
            _map = map;
        }

        public (int cost, List<Command> commands) GetRouteTo(Coordinate start, Coordinate goal)
        {
            var sw = Stopwatch.StartNew();

            var closed_set = new System.Collections.Generic.HashSet<Coordinate>();
            var came_from = new Dictionary<Coordinate, Coordinate>();
            var g_score = new Dictionary<Coordinate, float> { { start, 0 } };
            var f_score = new Dictionary<Coordinate, float> { { start, GetDistance(start, goal) } };
            var current = start;

            IPriorityQueue<Coordinate> open_set = new IntervalHeap<Coordinate>(new CoordComparer(f_score)) { start };
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

                foreach (var neighbor in ValidNeighbors(current))
                {
                    if (closed_set.Contains(neighbor))
                        continue;

                    var tentative_g_score = g_score[current] + MoveCost(current, neighbor);

                    if (!open_set_hash.Contains(neighbor) || tentative_g_score < g_score[neighbor])
                    {
                        g_score[neighbor] = tentative_g_score;
                        f_score[neighbor] = g_score[neighbor] + GetDistance(neighbor, goal);
                        open_set.Add(neighbor);
                        open_set_hash.Add(neighbor);
                        came_from[neighbor] = current;
                    }
                }
            }

            // fill out our unreachable points
            for (byte x = 0; x < _map.Resolution; x++)
            {
                for (byte y = 0; y < _map.Resolution; y++)
                {
                    for (byte z = 0; z < _map.Resolution; z++)
                    {
                        if (!closed_set.Contains(new Coordinate(x, y, z)))
                            _unreachable.Add(new Coordinate(x, y, z));
                    }
                }
            }

            if (sw.ElapsedMilliseconds > 1000)
                Log.Debug($"Failed to generate path path in {sw.ElapsedMilliseconds:N0} ms ({closed_set.Count:N0} closed, {came_from.Count:N0} examined)");

            return (0, null);
        }

        private (int cost, List<Command> commands) ReconstructPath(Dictionary<Coordinate, Coordinate> came_from, Coordinate start, Coordinate current)
        {
            List<Command> commands = new List<Command>();

            while (current != start)
            {
                var prev = came_from[current];
                commands.Add(new CommandSmove(new CoordinateDifference((sbyte)(current.x - prev.x), (sbyte)(current.y - prev.y), (sbyte)(current.z - prev.z))));

                current = prev;
            }

            commands.Reverse();

            return (commands.Count, commands);
        }

        public List<Coordinate> ValidNeighbors(Coordinate origin)
        {
            var list = new List<Coordinate>();

            // calculate all valid smoves

            for (sbyte i = -1; i >= -15; i--)
            {
                var d = new CoordinateDifference(i, 0, 0);
                var c = origin.Translate(d);
                if (_map.CalcSmove(origin, c) > 0)
                    list.Add(c);
                else
                    break;
            }

            for (sbyte i = 1; i <= 15; i++)
            {
                var d = new CoordinateDifference(i, 0, 0);
                var c = origin.Translate(d);
                if (_map.CalcSmove(origin, c) > 0)
                    list.Add(c);
                else
                    break;
            }

            for (sbyte i = -1; i >= -15; i--)
            {
                var d = new CoordinateDifference(0, i, 0);
                var c = origin.Translate(d);
                if (_map.CalcSmove(origin, c) > 0)
                    list.Add(c);
                else
                    break;
            }

            for (sbyte i = 1; i <= 15; i++)
            {
                var d = new CoordinateDifference(0, i, 0);
                var c = origin.Translate(d);
                if (_map.CalcSmove(origin, c) > 0)
                    list.Add(c);
                else
                    break;
            }

            for (sbyte i = -1; i >= -15; i--)
            {
                var d = new CoordinateDifference(0, 0, i);
                var c = origin.Translate(d);
                if (_map.CalcSmove(origin, c) > 0)
                    list.Add(c);
                else
                    break;
            }

            for (sbyte i = 1; i <= 15; i++)
            {
                var d = new CoordinateDifference(0, 0, i);
                var c = origin.Translate(d);
                if (_map.CalcSmove(origin, c) > 0)
                    list.Add(c);
                else
                    break;
            }

            return list;
        }

        private float MoveCost(Coordinate current, Coordinate neighbor)
        {
            return 1;
        }

        private float GetDistance(Coordinate start, Coordinate goal)
        {
            return start.Mlen(goal);
        }

        private class CoordComparer : IComparer<Coordinate>
        {
            private readonly Dictionary<Coordinate, float> _f_scores;

            public CoordComparer(Dictionary<Coordinate, float> f_scores)
            {
                _f_scores = f_scores;
            }

            public int Compare(Coordinate x, Coordinate y)
            {
                return _f_scores[x].CompareTo(_f_scores[y]);
            }
        }
    }
}