using System.Text;

namespace day10
{
    public class Part1
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

            // Find runner's inintal step, and take note of previous step to avoid doubling back, then send to path-finding BFS
            foreach (var (R, C) in directions)
            {
                (int R, int C) next = (runner.Row + R, runner.Col + C);

                if (OutOfBounds(next.R, next.C, sketch.Count - 1, sketch[0].Count - 1)) continue;
                if (sketch[next.R][next.C] == '.') continue;
                var pipe = sketch[next.R][next.C];
                var reverse = (R == 0 ? 0 : -R, C == 0 ? 0 : -C); // A check to see if we can get back from where we want to go to indicates a valid place to move to
                if (!pipes[pipe].Contains(reverse)) continue; // A check to see if we can get back from where we want to go to indicates a valid place to move to

                runner.Row = next.R;
                runner.Col = next.C;
                break;
            }

            result = BFS(
                runner,
                pipes,
                sketch,
                (start[0], start[1]),
                prev
            ) / 2;

            return result;
        }

        private static int BFS(
            MazeRunner runner,
            Dictionary<char, List<(int R, int C)>> pipes,
            List<List<char>> sketch,
            (int R, int C) start,
            int[] prev
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
                runner.Row = next.R;
                runner.Col = next.C;
                steps++;
                break;
            }

            // process next step
            steps += BFS(runner, pipes, sketch, start, prev);

            return steps;
        }

        private static bool OutOfBounds(int row, int col, int rowBound, int colBound)
        {
            return (0 > row || row > rowBound || 0 > col || col > colBound);
        }
    }
}