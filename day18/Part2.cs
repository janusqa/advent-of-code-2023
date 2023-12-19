using System.Text;

namespace day18
{
    public class Part2
    {
        public static long Result()
        {
            long result = 0;
            var instructions = new List<(char D, long L, string C)>();
            var grid = new List<(long R, long C)>();

            var directions = new Dictionary<char, (long R, long C)> {
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
                        instructions.Add((char.Parse(sections[0]), long.Parse(sections[1]), sections[2][2..(sections[2].Length - 1)]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            InitGrid(grid, instructions, directions);

            // We need to find the number of internal points in the polygon (using pick)
            // To calculate that we need the internal area of the polygon (using shoelace)
            // now we can add the number of internal points to the points on the border to 
            // get full area
            long boundaryPoints = instructions.Select(b => Convert.ToInt64(b.C[0..(b.C.Length - 1)], 16)).Sum();
            result = Pick(Shoelace(grid), boundaryPoints) + boundaryPoints;

            return result;
        }

        // convert instructions to a grid stored in a List of tuples
        // Each tuple represents how the other tuples around it interconnect
        private static void InitGrid(
            List<(long R, long C)> grid,
            List<(char D, long L, string C)> instructions,
            Dictionary<char, (long R, long C)> directions
        )
        {
            grid.Add((0, 0));
            foreach (var ((D, L, C), ix) in instructions.Select((ins, ix) => (ins, ix)))
            {
                char newD = C[^1] == '0' ? 'R' : C[^1] == '1' ? 'D' : C[^1] == '2' ? 'L' : 'U';
                long newL = Convert.ToInt64(C[0..(C.Length - 1)], 16);
                grid.Add((grid[^1].R + (directions[newD].R * newL), grid[^1].C + (directions[newD].C * newL)));
            }
        }

        // Shoelace Therom calculates the INTERNAL area witin a 
        // polygon. This excludes the border of the polygon itself
        private static long Shoelace(List<(long R, long C)> grid)
        {
            long left = 0;
            long right = 0;
            foreach (var ((R, C), ix) in grid.Select((rc, ix) => (rc, ix)))
            {
                if (ix + 1 < grid.Count)
                {
                    left += R * grid[ix + 1].C;
                    right += C * grid[ix + 1].R;
                }
            }

            return Math.Abs(left - right) / 2;
        }

        // Picks Theroem: Area = (Boundary_Points / 2) + Internal_Points - 1
        // We are trying to get the internal points here to add back to the 
        // boundary points. Internal_Points = Area - (Boundary_Points / 2) + 1
        private static long Pick(long A, long B)
        {
            return A - (B / 2) + 1;
        }
    }
}