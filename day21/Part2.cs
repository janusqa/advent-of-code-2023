using System.Globalization;
using System.Text;

namespace day21
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;

            var map = new List<List<char>>();
            (int R, int C) start = (0, 0);

            try
            {
                using (StreamReader reader = new StreamReader(@"./day21/input_test.txt", Encoding.UTF8))
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

            int RUBOUND = map.Count;
            int CUBOUND = map[0].Count;
            var directions = new List<(int R, int C)> { (-1, 0), (0, 1), (1, 0), (0, -1) };
            var lastSeen = new HashSet<(int R, int C)>();
            var steps = new Queue<HashSet<(int R, int C)>>();
            int stepsTaken = 0;
            int stepsToTake = 500;


            steps.Enqueue([start]);
            while (steps.Count > 0 && stepsTaken < stepsToTake)
            {
                var possibilities = steps.Dequeue();
                var reach = new HashSet<(int R, int C)>();
                foreach (var possibility in possibilities)
                {
                    foreach (var direction in directions)
                    {
                        var newR = possibility.R + direction.R;
                        var newC = possibility.C + direction.C;
                        if (map[Wrap(newR, RUBOUND)][Wrap(newC, CUBOUND)] != '#')
                        {
                            reach.Add((newR, newC));
                        }
                    }
                }
                steps.Enqueue(reach);
                lastSeen = new HashSet<(int R, int C)>(reach);
                int numSteps = Cycle(map, reach);
                if (numSteps != -1) { Console.WriteLine($"found a match! -> {numSteps}"); break; }
                stepsTaken++;
            }

            Print(map, lastSeen);
            result = lastSeen.Count;
            return result;
        }

        public static int Wrap(int index, int wrapAt) => (index % wrapAt + wrapAt) % wrapAt;

        public static int Cycle(List<List<char>> map, HashSet<(int R, int C)> reach)
        {
            int RUBOUND = map.Count;
            int CUBOUND = map[0].Count;
            var seen = new HashSet<string>();

            for (int k = 0; k < 5; k++)
            {
                StringBuilder key = new StringBuilder("");
                for (int i = RUBOUND * k; i < RUBOUND * (k + 1); i++)
                {
                    for (int j = CUBOUND * k; j < CUBOUND * (k + 1); j++)
                    {
                        key.Append(reach.Contains((i, j)) ? 'O' : map[Wrap(i, RUBOUND)][Wrap(j, CUBOUND)]);
                    }
                }

                string skey = key.ToString();
                if (!skey.Any(p => p == 'O')) continue;
                if (seen.Contains(skey))
                {
                    Console.WriteLine(k);
                    return seen.Sum(grid => grid.Count(c => c == 'O')); ;
                }
                seen.Add(skey);
            }
            return -1;
        }

        public static void Print(List<List<char>> map, HashSet<(int R, int C)> reach)
        {
            int RUBOUND = map.Count;
            int CUBOUND = map[0].Count;

            int k = 0;
            for (int i = RUBOUND * k; i < RUBOUND * (k + 4); i++)
            {
                for (int j = CUBOUND * k; j < CUBOUND * (k + 4); j++)
                {
                    Console.Write(reach.Contains((i, j)) ? 'O' : map[Wrap(i, map.Count)][Wrap(j, map[0].Count)]);
                    if (j % map[0].Count == map[0].Count - 1 && j > 0) Console.Write(" ");
                }
                if (i % map.Count == map.Count - 1) Console.WriteLine();
                Console.WriteLine();
            }
        }
    }

}