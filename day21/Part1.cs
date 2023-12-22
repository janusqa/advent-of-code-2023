using System.Text;

namespace day21
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;

            var map = new List<List<char>>();
            (int R, int C) start = (0, 0);

            try
            {
                using (StreamReader reader = new StreamReader(@"./day21/input.txt", Encoding.UTF8))
                {
                    string? line;
                    int rowCount = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null)
                        {
                            var row = line.ToCharArray().ToList();
                            map.Add([.. row]);
                            int startC = row.IndexOf('S');
                            if (startC != -1) start = (rowCount, startC);
                            rowCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            int RUBOUND = map.Count - 1;
            int CUBOUND = map[0].Count - 1;
            var directions = new List<(int R, int C)> { (-1, 0), (0, 1), (1, 0), (0, -1) };
            var steps = new Queue<HashSet<(int R, int C)>>();
            int stepsTaken = 0;

            steps.Enqueue([start]);
            while (steps.Count > 0 && stepsTaken < 64)
            {
                var possibilities = steps.Dequeue();
                var reach = new HashSet<(int R, int C)>();
                foreach (var possibility in possibilities)
                {
                    foreach (var direction in directions)
                    {
                        var newR = possibility.R + direction.R;
                        var newC = possibility.C + direction.C;
                        if (0 <= newR && newR <= RUBOUND && 0 <= newC && newC <= CUBOUND && map[newR][newC] != '#')
                        {
                            reach.Add((newR, newC));
                        }
                    }
                }
                steps.Enqueue(reach);
                result = reach.Count;
                stepsTaken++;
            }
            return result;
        }
    }
}