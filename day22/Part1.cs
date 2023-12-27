using System.Text;

namespace day22
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            var bricks = new List<((int x, int y, int z) s1, (int x, int y, int z) s2)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day22/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null)
                        {
                            var sides = line.Split("~", StringSplitOptions.RemoveEmptyEntries);
                            var s1 = sides[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                            var s2 = sides[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                            bricks.Add(((s1[0], s1[1], s1[2]), (s2[0], s2[1], s2[2])));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            bricks = [.. bricks.OrderBy(b => Math.Min(b.s1.z, b.s2.z))];

            foreach (var (brick, idx) in bricks.Select((b, idx) => (b, idx)).ToList())
            {
                if (OnGround(brick)) continue;
                int newPosition = Settle(brick, bricks);

                if (brick.s2.z >= brick.s1.z)
                {
                    bricks[idx] = ((brick.s1.x, brick.s1.y, newPosition), (brick.s2.x, brick.s2.y, newPosition + (brick.s2.z - brick.s1.z)));
                }
                else
                {
                    bricks[idx] = ((brick.s1.x, brick.s1.y, newPosition + (brick.s2.z - brick.s1.z)), (brick.s2.x, brick.s2.y, newPosition));
                }
            }

            foreach (var brick in bricks)
            {
                if (Supporting(brick, bricks).All(b => SupportedBy(b, bricks) > 1)) result++;
            }

            return result;
        }

        private static int Settle(
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick,
            List<((int x, int y, int z) s1, (int x, int y, int z) s2)> bricks
        )
        {
            var below = bricks
                .Where(b => Math.Max(b.s1.z, b.s2.z) < Math.Min(brick.s1.z, brick.s2.z) && Overlapping(b, brick))
                .Select(b => Math.Max(b.s1.z, b.s2.z));

            return below.Any() ? below.Max() + 1 : 0;
        }

        private static bool Touching(
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick1,
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick2
        ) => Overlapping(brick1, brick2) &&
            ((Math.Abs(Math.Max(brick1.s1.z, brick1.s2.z) - Math.Min(brick2.s1.z, brick2.s2.z)) == 1) ||
            (Math.Abs(Math.Max(brick2.s1.z, brick2.s2.z) - Math.Min(brick1.s1.z, brick1.s2.z)) == 1));

        private static bool Overlapping(
        ((int x, int y, int z) s1, (int x, int y, int z) s2) brick1,
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick2
        ) => (!(Math.Max(brick1.s1.x, brick1.s2.x) < Math.Min(brick2.s1.x, brick2.s2.x) ||
             Math.Max(brick2.s1.x, brick2.s2.x) < Math.Min(brick1.s1.x, brick1.s2.x))) &&
             (!(Math.Max(brick1.s1.y, brick1.s2.y) < Math.Min(brick2.s1.y, brick2.s2.y) ||
             Math.Max(brick2.s1.y, brick2.s2.y) < Math.Min(brick1.s1.y, brick1.s2.y)));

        private static List<((int x, int y, int z) s1, (int x, int y, int z) s2)> Supporting(
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick,
            List<((int x, int y, int z) s1, (int x, int y, int z) s2)> bricks
        ) => bricks.Where(b => Touching(brick, b) && Math.Min(b.s1.z, b.s2.z) > Math.Max(brick.s1.z, brick.s2.z)).ToList();

        private static int SupportedBy(
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick,
            List<((int x, int y, int z) s1, (int x, int y, int z) s2)> bricks
        ) => bricks.Where(b => Touching(brick, b) && Math.Min(brick.s1.z, brick.s2.z) > Math.Max(b.s1.z, b.s2.z)).Count();

        private static bool OnGround(((int x, int y, int z) s1, (int x, int y, int z) s2) brick) => Math.Min(brick.s1.z, brick.s2.z) == 1;
    }
}
