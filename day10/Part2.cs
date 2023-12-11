using System.Text;

namespace day10
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;

            var pipes = new Dictionary<char, List<(int R, int C)>>() {
                {'|', [(-1, 0), (1, 0)]},
                {'-', [(0, -1), (0, 1)]},
                {'L', [(-1, 0), (0, 1)]},
                {'J', [(-1, 0), (0, -1)]},
                {'7', [(1, 0), (0, -1)]},
                {'F', [(1, 0), (0, 1)]},
            };
            var directions = new List<(int R, int C)>() { (-1, 0), (0, 1), (1, 0), (0, -1) };
            var sketch = new List<List<char>>();
            var start = new int[2];
            var pipeLoop = new HashSet<(int R, int C)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day10/input.txt", Encoding.UTF8))
                {
                    string? line;
                    int row = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains('S'))
                        {
                            start[0] = row;
                            start[1] = line.IndexOf('S');
                        }
                        sketch.Add([.. line.ToCharArray()]);
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Console.WriteLine(string.Join(", ", position.Select(s => s)));
            // foreach (var line in sketch)
            // {
            //     Console.WriteLine(string.Join("", line));
            // }

            var runner = new MazeRunner(start[0], start[1]); // runner located at start
            var prev = new int[2] { runner.Row, runner.Col };
            pipeLoop.Add((runner.Row, runner.Col));

            // Find runner's inintal step, and take note of previous step to avoid doubling back, then send to path-finding BFS
            // Also Identify which pipe segment 'S' really is, by identify which two directions go out from it, match it back 
            // with the pipes dictionary and deduce it.
            var sDirections = new List<(int R, int C)>();
            foreach (var (R, C) in directions)
            {
                (int R, int C) next = (start[0] + R, start[1] + C);

                if (OutOfBounds(next.R, next.C, sketch.Count - 1, sketch[0].Count - 1)) continue;
                if (sketch[next.R][next.C] == '.') continue;
                var pipe = sketch[next.R][next.C];
                var reverse = (R == 0 ? 0 : -R, C == 0 ? 0 : -C); // A check to see if we can get back from where we want to go to indicates a valid place to move to
                if (!pipes[pipe].Contains(reverse)) continue; // A check to see if we can get back from where we want to go to indicates a valid place to move to

                runner.Row = next.R;
                runner.Col = next.C;

                sDirections.Add((R, C));
            }

            // deduce what 'S' is and overwrite S with it's correct pipe represntation
            char S = pipes.Where(p => p.Value.Contains(sDirections[0]) && p.Value.Contains(sDirections[1])).Select(p => p.Key).First();
            // Console.Write($"{string.Join(", ", sDirections)} -- {S}");
            sketch[start[0]][start[1]] = S;

            int steps = BFS(
                runner,
                pipes,
                sketch,
                (start[0], start[1]),
                prev,
                pipeLoop
            ) / 2;


            // Console.WriteLine(string.Join(" ", pipeLoop.Select(p => p)));

            // Print grid
            // for (int row = 0; row < sketch.Count; row++)
            // {
            //     for (int col = 0; col < sketch[row].Count; col++)
            //     {
            //         Console.Write(pipeLoop.Contains((row, col)) ? sketch[row][col] : ".");
            //     }
            //     Console.WriteLine();
            // }

            // Here we use ray tracing to determine if a point is inside/outside the pipe enclousure.
            // If an odd number of pipes are to the left of it then its inside the enclousure.
            // If an even number of pipes are to the left of it then its outside the enclousure.
            // that is if looking left from a point and we see an odd number of points we are inside otherwise we are outside.
            // We have to decide if we are looking along the top or bottom of a row to decide if we are crossing a pipe.
            // If looking along top, pipes we are interested in are 'L', '|' and 'J' because we cannot squeeze past them as they connect to pipe above
            // If looking along bottom, pipes we are interested in are '7', '|' and 'F' because we cannot squeeze past them as they connect to pipe below
            // We can use either or groups of pipes to check for and it should work.
            int enclosed = 0;
            for (int row = 0; row < sketch.Count; row++)
            {
                for (int col = 0; col < sketch[row].Count; col++)
                {
                    if (pipeLoop.Contains((row, col))) continue;
                    if (pipeLoop
                            .Where(
                                pipe => (pipe.R == row) &&
                                (pipe.C < col) && (sketch[pipe.R][pipe.C] == 'F' || sketch[pipe.R][pipe.C] == '7' || sketch[pipe.R][pipe.C] == '|')
                            ).Count() % 2 == 0
                        ) continue;

                    // Console.WriteLine($"{row},{col} {string.Join(" ", pipeLoop.Where(pipe => (pipe.R == row) && (pipe.C < col)))}");
                    enclosed++;

                }
            }

            result = enclosed;

            return result;
        }

        private static int BFS(
            MazeRunner runner,
            Dictionary<char, List<(int R, int C)>> pipes,
            List<List<char>> sketch,
            (int R, int C) start,
            int[] prev,
            HashSet<(int R, int C)> pipeLoop
        )
        {
            // Console.WriteLine($"Start: {start.R},{start.C} | Runner: {runner.Row},{runner.Col} | Prev: {string.Join(",", prev)}");

            if (runner.Row == start.R && runner.Col == start.C) return 1;

            int steps = 0;
            var pipe = sketch[runner.Row][runner.Col];

            // Find next step
            foreach (var (R, C) in pipes[pipe])
            {
                (int R, int C) next = (runner.Row + R, runner.Col + C);

                if (OutOfBounds(next.R, next.C, sketch.Count - 1, sketch[0].Count - 1)) continue;
                if (sketch[next.R][next.C] == '.') continue;
                if (next.R == prev[0] && next.C == prev[1]) continue; // we are trying to go back. We should not.

                prev[0] = runner.Row;
                prev[1] = runner.Col;
                pipeLoop.Add((runner.Row, runner.Col));
                runner.Row = next.R;
                runner.Col = next.C;
                steps++;
                break;

            }

            // process next step
            steps += BFS(runner, pipes, sketch, start, prev, pipeLoop);

            return steps;
        }

        private static bool OutOfBounds(int row, int col, int rowBound, int colBound)
        {
            return (0 > row || row > rowBound || 0 > col || col > colBound);
        }
    }
}