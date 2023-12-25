using System.Text;

namespace day23
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;

            var map = new List<List<char>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day23/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null)
                        {
                            map.Add([.. line.ToCharArray()]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            var directions = new Dictionary<char, (int R, int C)> {
                {'U', (-1,0)},
                {'R', (0,1)},
                {'D', (1,0)},
                {'L', (0,-1)},
            };
            var slopes = new Dictionary<char, char> {
                {'^', 'U'},
                {'>', 'R'},
                {'V', 'D'},
                {'<', 'L'},
            };
            int RUBound = map.Count - 1;
            int CUBound = map[0].Count - 1;
            (int R, int C) start = (0, 1);
            (int R, int C) end = (RUBound, CUBound - 1);
            var nodes = new List<(int R, int C)> { start, end };
            var adjacencyList = new Dictionary<(int R, int C), Dictionary<(int R, int C), int>>();

            // Step 1: reduce the graph (edge contraction) and it should be an aycyclic graph or this will not work
            // Step 2: After edge contraction build an adjancy list then brute forute force with DFS keeping track of higest steps
            Graph(nodes, adjacencyList, map, directions);

            // foreach (var node in adjacencyList)
            // {
            //     Console.WriteLine($"{node.Key}: {string.Join(", ", node.Value.Select(a => $"{string.Join(" ", $"{a.Key}: {a.Value}")}"))}");
            // }
            // Console.WriteLine();

            result = Dfs((start, 0), end, adjacencyList, []);

            return result;
        }

        private static void Graph(
            List<(int R, int C)> nodes,
            Dictionary<(int R, int C), Dictionary<(int R, int C), int>> adjacencyList,
            List<List<char>> map,
            Dictionary<char, (int R, int C)> directions
        )
        {
            int RUBound = map.Count - 1;
            int CUBound = map[0].Count - 1;

            // Node compression
            foreach (var (row, rIdx) in map.Select((r, rIdx) => (r, rIdx)))
            {
                foreach (var (col, cIdx) in row.Select((c, cIdx) => (c, cIdx)))
                {
                    if (col == '#') continue;

                    int neighbors = 0;
                    foreach (var direction in directions)
                    {
                        int toR = rIdx + direction.Value.R;
                        int toC = cIdx + direction.Value.C;

                        if (0 > toR || toR > RUBound || 0 > toC || toC > CUBound || map[toR][toC] == '#') continue;

                        neighbors++;
                    }
                    if (neighbors > 2) nodes.Add((rIdx, cIdx));
                }
            }

            // build adjency list from Nodes after compression
            foreach (var node in nodes)
            {
                var walk = new Queue<((int R, int C) L, int S)>();
                var visited = new HashSet<(int R, int C)>();
                adjacencyList.Add(node, []);

                walk.Enqueue((node, 0));
                while (walk.Count > 0)
                {
                    var tile = walk.Dequeue();

                    if (visited.Contains(tile.L)) continue;
                    visited.Add(tile.L);

                    // if it is one of our junction nodes, i.e. a node with more than two choices 
                    // since it cannot step back to where its comming from.
                    if (tile.L != node && nodes.Contains(tile.L))
                    {
                        if (adjacencyList.TryGetValue(node, out Dictionary<(int R, int C), int>? neighbors))
                        {
                            neighbors.Add(tile.L, tile.S);
                        }
                        else
                        {
                            adjacencyList.Add(node, new Dictionary<(int R, int C), int> { { tile.L, tile.S } });
                        }
                    }
                    else
                    {
                        foreach (var direction in new List<(int R, int C)> { (-1, 0), (1, 0), (0, -1), (0, 1) })
                        {
                            int toR = tile.L.R + direction.R;
                            int toC = tile.L.C + direction.C;

                            if (0 > toR || toR > RUBound || 0 > toC || toC > CUBound || map[toR][toC] == '#' || visited.Contains((toR, toC))) continue;
                            walk.Enqueue(((toR, toC), tile.S + 1));
                        }
                    }
                }
            }
        }

        private static int Dfs(
            ((int R, int C) L, int S) node,
            (int R, int C) end,
            Dictionary<(int R, int C), Dictionary<(int R, int C), int>> adjanceyList,
            HashSet<(int R, int C)> visited
        )
        {
            if (node.L == end) return 0;

            // cycle avoidance. This node can have neighbours who has neigbours that are this node causing as cycle ugh.
            // use visited to temprorily makde sure it is not repeatedly sent to DFS causing a loop of death
            // remember to remove it after checking it's neighbours.
            visited.Add(node.L);

            int steps = int.MinValue;

            foreach (var neighbor in adjanceyList[node.L])
            {
                if (!visited.Contains(neighbor.Key)) // cycle avoidance
                {
                    steps = Math.Max(Dfs((neighbor.Key, neighbor.Value), end, adjanceyList, visited) + adjanceyList[node.L][neighbor.Key], steps);
                }
            }

            visited.Remove(node.L); // cycle avoidance

            return steps;
        }
    }
}