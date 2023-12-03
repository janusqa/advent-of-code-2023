using System.Text;
using System.Text.RegularExpressions;

namespace day02
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;
            var cubes = new Dictionary<string, int> {
                {"red", 0},
                {"green", 0},
                {"blue", 0}
            };
            var rx = new Regex(@"(\d{1,}) (red|green|blue)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
                                        cubes[matches[i].Groups[2].Value] = numCubes;
                                    }
                                }
                            }
                        }
                        result += cubes["red"] * cubes["green"] * cubes["blue"];
                        cubes.Keys.ToList().ForEach(x => cubes[x] = 0);
                    }
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