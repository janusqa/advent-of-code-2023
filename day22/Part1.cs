using System.Security.Cryptography.X509Certificates;
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
                using (StreamReader reader = new StreamReader(@"./day22/input_test.txt", Encoding.UTF8))
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

            // foreach (var brick in bricks)
            // {
            //     Console.WriteLine(string.Join(", ", brick));
            // }

            bricks = [.. bricks.OrderBy(b => Math.Min(b.s1.z, b.s2.z))];

            foreach (var brick in bricks)
            {
                if (brick.s1.z == 1) continue;


            }



            return result;
        }

        private static void Falling(
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick,
            List<((int x, int y, int z) s1, (int x, int y, int z) s2)> bricks
        )
        {



        }

        private static bool Touching(
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick1,
            ((int x, int y, int z) s1, (int x, int y, int z) s2) brick2
        )
        {
            (Math.Abs(brick1.s1.z - brick2.s1.z) == 1 && true)
            return true;
        }
    }
}
