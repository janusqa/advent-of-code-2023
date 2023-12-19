using System.Text;

namespace day18
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            var instructions = new List<(char D, int L, string C)>();
            var grid = new List<((int R, int C) P, char H)>();

            var directions = new Dictionary<char, (int R, int C)> {
                {'U', (-1,0)},
                {'R', (0,1)},
                {'D', (1,0)},
                {'L', (0,-1)}
            };

            try
            {
                using (StreamReader reader = new StreamReader(@"./day18/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sections = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        instructions.Add((char.Parse(sections[0]), int.Parse(sections[1]), sections[2][1..(sections[2].Length - 1)]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            InitGrid(grid, instructions, directions);

            (int RL, int RU, int CL, int CU) boundaries = Boundaries(grid);

            int enclosed = 0;
            for (int row = boundaries.RL; row <= boundaries.RU; row++)
            {
                for (int col = boundaries.CL; col <= boundaries.CU; col++)
                {
                    if (grid.FindIndex(h => h.P.R == row && h.P.C == col) != -1) continue;
                    if (grid
                            .Where(hole => (hole.P.R == row) && (hole.P.C < col))
                            .Select(hole => hole.H).Where(hole => hole == 'F' || hole == '7' || hole == '|')
                            .Count() % 2 == 0
                        ) continue;
                    enclosed++;
                }
            }

            result = grid.Count + enclosed;

            return result;
        }

        // Determine what a hole looks like
        private static char Hole(char h, char pd, char d)
        {
            char hole = '\0';

            if (h == '.')
            {
                hole = (d == 'R' || d == 'L') ? '-' : '|';
            }
            else
            {
                if (pd == 'R' && d == 'D' || pd == 'U' && d == 'L') hole = '7';
                if (pd == 'L' && d == 'D' || pd == 'U' && d == 'R') hole = 'F';
                if (pd == 'D' && d == 'R' || pd == 'L' && d == 'U') hole = 'L';
                if (pd == 'D' && d == 'L' || pd == 'R' && d == 'U') hole = 'J';
            }

            return hole;
        }

        // Deduce the boundaries of the grid
        private static (int RL, int RU, int CL, int CU) Boundaries(List<((int R, int C) P, char H)> grid)
        {
            int RL = 0;
            int RU = 0;
            int CL = 0;
            int CU = 0;

            RL = grid.Select(h => h.P.R).Min();
            RU = grid.Select(h => h.P.R).Max();
            CL = grid.Select(h => h.P.C).Min();
            CU = grid.Select(h => h.P.C).Max();

            return (RL, RU, CL, CU);
        }

        // convert instructions to a grid stored in a List of tuples
        // Each tuple represents how the other tuples around it interconnect
        private static void InitGrid(
            List<((int R, int C) P, char H)> grid,
            List<(char D, int L, string C)> instructions,
            Dictionary<char, (int R, int C)> directions
        )
        {
            (int R, int C) cp = (0, 0);
            char pd = instructions[0].D;
            char sp = instructions[0].D;
            foreach (var ((D, L, C), ix) in instructions.Select((ins, ix) => (ins, ix)))
            {
                {
                    for (int i = 0; i < L + 1; i++)
                    {
                        if (!grid.Where(h => h.P.R == cp.R && h.P.C == cp.C).Any())
                        {
                            grid.Add(((cp.R, cp.C), Hole('.', pd, D)));
                        }
                        else
                        {
                            int gIdx = grid.FindIndex(h => h.P.R == cp.R && h.P.C == cp.C);
                            if (gIdx != -1) grid[gIdx] = ((cp.R, cp.C), Hole('\0', pd, D));
                        }
                        if (i < L) cp = (cp.R + directions[D].R, cp.C + directions[D].C);
                    }
                    pd = D;
                    if (ix == instructions.Count - 1)
                    {
                        int gIdx = grid.FindIndex(h => h.P.R == cp.R && h.P.C == cp.C);
                        if (gIdx != -1) grid[gIdx] = ((cp.R, cp.C), Hole('\0', D, sp));
                    }
                }
            }
        }
    }
}