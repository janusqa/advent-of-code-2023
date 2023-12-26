using System.Text;

namespace day21
{
    public class Part2
    {
        public static long Result()
        {
            long result = 0;

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

            result = GetReach(start, 26501365, map);

            return result;
        }

        private static long GetReach((int R, int C) start, int numSteps, List<List<char>> map)
        {
            int RUBOUND = map.Count;
            int CUBOUND = map[0].Count;
            var directions = new List<(int R, int C)> { (-1, 0), (0, 1), (1, 0), (0, -1) };
            var steps = new HashSet<(int R, int C)> { start };
            var prevPlotsReached = 0;
            (int FirstLevel, int SecondLevel) delta = (0, 0);
            var quadraticSequence = new List<int>();
            var normalizedNumSteps = numSteps % map.Count;

            foreach (var stepsTaken in Enumerable.Range(1, numSteps))
            {
                var reach = new HashSet<(int R, int C)>();
                foreach (var step in steps)
                {
                    foreach (var (R, C) in directions)
                    {
                        var newR = step.R + R;
                        var newC = step.C + C;
                        if (map[Wrap(newR, RUBOUND)][Wrap(newC, CUBOUND)] != '#')
                        {
                            reach.Add((newR, newC));
                        }
                    }
                }

                steps = reach;

                if (stepsTaken >= normalizedNumSteps && (stepsTaken - normalizedNumSteps) % map.Count == 0)
                {
                    quadraticSequence.Add(reach.Count);
                    var deltaFirstLevelCurrent = reach.Count - prevPlotsReached;
                    if (deltaFirstLevelCurrent - delta.FirstLevel - delta.SecondLevel == 0) break;
                    prevPlotsReached = reach.Count;
                    delta = (deltaFirstLevelCurrent, deltaFirstLevelCurrent - delta.FirstLevel);
                }
            }

            long a = delta.SecondLevel / 2;
            long b = (quadraticSequence[1] - quadraticSequence[0]) - (3 * a);
            long c = quadraticSequence[0] - a - b;
            long n = 1 + (numSteps / map.Count);

            return (a * (n * n)) + (b * n) + c;
        }

        private static int Wrap(int index, int wrapAt) => (index % wrapAt + wrapAt) % wrapAt;
    }

}