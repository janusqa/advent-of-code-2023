using System.Text;

namespace day23
{
    public class Part1
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
            // Step 2: After edge contraction build an adjancy list for Step 3
            AcyclicGraph(nodes, adjacencyList, map, directions, slopes);

            // foreach (var node in adjacencyList)
            // {
            //     Console.WriteLine($"{node.Key}: {string.Join(", ", node.Value.Select(a => $"{string.Join(" ", $"{a.Key}: {a.Value}")}"))}");
            // }
            // Console.WriteLine();

            // Step 3: Sort the adjency list topologically.  This will make finding distances from source node more reliable.
            var sorted = new Stack<(int R, int C)>();
            var sortedVisited = new HashSet<(int R, int C)>();
            foreach (var node in adjacencyList)
            {
                if (!sortedVisited.Contains(node.Key)) TopologicalSort(node.Key, adjacencyList, sortedVisited, sorted);
            }

            // Step 4: go thru the toplogically sorted adjancy list and calculate the distances from each node to the source node
            var dist = new Dictionary<(int R, int C), int>();
            foreach (var node in adjacencyList)
            {
                dist.Add(node.Key, int.MinValue);
            }
            dist[start] = 0;
            while (sorted.Count > 0)
            {
                var vertex = sorted.Pop();
                if (dist[vertex] != int.MinValue)
                {
                    foreach (var neighbor in adjacencyList[vertex])
                    {
                        if (dist[neighbor.Key] < dist[vertex] + neighbor.Value)
                        {
                            dist[neighbor.Key] = dist[vertex] + neighbor.Value;
                        }
                    }
                }
            }

            // foreach (var node in dist)
            // {
            //     Console.WriteLine($"{node.Key}: {node.Value}");
            // }

            result = dist[end];

            return result;
        }

        private static void AcyclicGraph(
            List<(int R, int C)> nodes,
            Dictionary<(int R, int C), Dictionary<(int R, int C), int>> adjacencyList,
            List<List<char>> map,
            Dictionary<char, (int R, int C)> directions,
            Dictionary<char, char> slopes
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
            var walkDirections = new Dictionary<char, List<(int R, int C)>> {
                {'^', [(-1, 0)]},
                {'v', [(1, 0)]},
                {'<', [(0, -1)]},
                {'>', [(0, 1)]},
                {'.', [(-1, 0), (1, 0), (0, -1), (0, 1)]},
            };

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
                        foreach (var direction in walkDirections[map[tile.L.R][tile.L.C]])
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

        private static void TopologicalSort(
            (int R, int C) node,
            Dictionary<(int R, int C), Dictionary<(int R, int C), int>> adjanceyList,
            HashSet<(int R, int C)> visited,
            Stack<(int R, int C)> sorted
        )
        {
            visited.Add(node);
            foreach (var neighbour in adjanceyList[node])
            {
                if (!visited.Contains(neighbour.Key)) TopologicalSort(neighbour.Key, adjanceyList, visited, sorted);
            }
            sorted.Push(node);
        }
    }
}