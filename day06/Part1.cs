
using System.Text;

namespace day06
{
    public class Part1
    {
        public static int Result()
        {
            int result = 1;

            var games = new List<(int Time, int Distance)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day06/input.txt", Encoding.UTF8))
                {

                    var gamesData = reader.ReadToEnd()
                                    .Split("\n")
                                    .Select(line =>
                                                line.Split(": ")[1]
                                                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                                .Select(Int32.Parse)
                                                .ToList())
                                    .ToList();
                    games = gamesData[0].Zip(gamesData[1], (time, distance) => (time, distance)).ToList();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var (Time, Distance) in games)
            {
                int wins = 0;

                for (long press = 0; press < Time; press++)
                {
                    if ((Time - press) * press > Distance)
                    {
                        wins++;
                    }
                }

                result *= wins;
            }

            return result;
        }
    }
}