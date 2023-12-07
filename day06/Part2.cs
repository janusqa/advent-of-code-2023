
using System.Text;

namespace day06
{
    public class Part2
    {
        public static long Result()
        {
            long result = 1;

            var games = new List<(long Time, long Distance)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day06/input.txt", Encoding.UTF8))
                {

                    var gamesData = reader.ReadToEnd()
                                    .Split("\n")
                                    .Select(line =>
                                                line.Split(": ")[1]
                                                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                                .Select(long.Parse)
                                                .ToList())
                                    .ToList();
                    games = gamesData[0].Zip(gamesData[1], (time, distance) => (time, distance)).ToList();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            long time = long.Parse(string.Join("", games.Select(g => g.Time)));
            long distance = long.Parse(string.Join("", games.Select(g => g.Distance))); ;

            long wins = 0;
            for (long press = 0; press < time; press++)
            {
                if ((time - press) * press > distance)
                {
                    wins++;
                }
            }

            result *= wins;

            return result;
        }
    }
}