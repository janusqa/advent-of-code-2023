using System.Text;
using System.Text.RegularExpressions;

namespace day02
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            var cubes = new Dictionary<string, int> {
                {"red",12},
                {"green",13},
                {"blue", 14}
            };
            var rx = new Regex(@"(\d{1,}) (red|green|blue)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            int gameId = 1;
            int invalidGames = 0;

            // we will identify the games that are invalid and sum up there IDs
            // we will then subtract that value from the sum of ALL game IDs
            // we can use gausses formula to tally up a sum of all the game IDs
            try
            {
                using (StreamReader reader = new StreamReader(@"./day02/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        MatchCollection matches = rx.Matches(line);
                        if (matches.Count > 0)
                        {
                            for (int i = 0; i < matches.Count; i++)
                            {
                                if (Int32.TryParse(matches[i].Groups[1].Value, out int numCubes))
                                {
                                    if (numCubes > cubes[matches[i].Groups[2].Value])
                                    {
                                        invalidGames += gameId;
                                        break;
                                    }
                                }
                            }
                        }
                        gameId++;
                    }
                    // result is equals to the sum of all games (calculated using gausses formula) minus invalid games
                    result = (((gameId - 1 + 1) * (gameId - 1)) / 2) - invalidGames;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return result;
        }

    }
}