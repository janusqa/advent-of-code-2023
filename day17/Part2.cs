using System.Text;

namespace day17
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;

            var map = new List<List<int>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day17/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        map.Add([.. line.Select(c => int.Parse(c.ToString()))]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var row in map)
            // {
            //     Console.WriteLine(string.Join("", row));
            // }

            result = Dijkstra(map);

            return result;
        }

        // Dijkstra's Algorithmn
        public static int Dijkstra(List<List<int>> map)
        {
            int uRBound = map.Count - 1;
            int uCBound = map[0].Count - 1;

            var directions = new Dictionary<char, (int R, int C)> {
                {'U', (-1,0)},
                {'R', (0,1)},
                {'D', (1,0)},
                {'L', (0,-1)},
            };

            (int R, int C) destination = (uRBound, uCBound);

            var movements = new PriorityQueue<((int R, int C) L, char D, int S, int H), int>();
            var visited = new Dictionary<(int R, int C, char D, int S), int>();

            movements.Enqueue(((0, 0), '-', 0, 0), 0);

            while (movements.Count > 0)
            {
                var (L, D, S, H) = movements.Dequeue();
                // Console.WriteLine($"({(L.R > 9 ? L.R : " " + L.R.ToString())},{(L.C > 9 ? L.C : " " + L.C.ToString())}): {D} {S} {H}");

                if (!visited.ContainsKey((L.R, L.C, D, S)))
                {
                    visited[(L.R, L.C, D, S)] = H;
                    if (L == destination && S >= 3) break; // must arrive at destination with 4 or greater steps

                    foreach (var direction in directions)
                    {
                        int toR = L.R + direction.Value.R;
                        int toC = L.C + direction.Value.C;

                        if (0 > toR || toR > uRBound || 0 > toC || toC > uCBound) continue; // prevent out of bounds 
                        bool isValidDirection = directions.TryGetValue(D, out (int R, int C) d);
                        if (isValidDirection && (d.R + direction.Value.R, d.C + direction.Value.C) == (0, 0)) continue; // prevent moving backwards
                        if (isValidDirection && D != direction.Key && S < 3) continue; // must take 4 steps before turning is possible
                        int steps = D == direction.Key ? S + 1 : 0; // if you move in same direction as previous move increment steps else reset it
                        if (steps > 9) continue;

                        int priority = H + map[toR][toC] < int.MaxValue ? H + map[toR][toC] : int.MaxValue;
                        movements.Enqueue(((toR, toC), direction.Key, steps, priority), priority);
                    }
                }
            }

            return visited.Where(e => (e.Key.R, e.Key.C) == destination && e.Key.S >= 3).Min().Value;
        }
    }
}