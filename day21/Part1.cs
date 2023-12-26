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
            var steps = new HashSet<(int R, int C)> { start };

            foreach (var _ in Enumerable.Range(1, 64))
            {
                var reach = new HashSet<(int R, int C)>();
                foreach (var step in steps)
                {
                    foreach (var (R, C) in directions)
                    {
                        var newR = step.R + R;
                        var newC = step.C + C;
                        if (0 <= newR && newR <= RUBOUND && 0 <= newC && newC <= CUBOUND && map[newR][newC] != '#')
                        {
                            reach.Add((newR, newC));
                        }
                    }
                }
                steps = reach;
            }

            result = steps.Count;
            return result;
        }
    }
}