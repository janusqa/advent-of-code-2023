using System.Text;
using System.Text.RegularExpressions;

namespace day01
{
    public class Part2
    {
        public static int Result()
        {
            var stringToNumberDict = new Dictionary<string, int> {
                {"one", 1},
                {"two", 2},
                {"three", 3},
                {"four", 4},
                {"five", 5},
                {"six", 6},
                {"seven", 7},
                {"eight", 8},
                {"nine", 9}
            };

            int result = 0;

            var rx = new Regex(@"(?=(\d|one|two|three|four|five|six|seven|eight|nine))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            try
            {
                using (StreamReader reader = new StreamReader(@"./day01/input.txt", Encoding.UTF8))
                {
                    string? line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        MatchCollection matches = rx.Matches(line);
                        var calibrationNumber = 0;

                        if (matches.Count > 0)
                        {
                            var first = Int32.TryParse(matches[0].Groups[1].Value, out int c1) ? c1 : stringToNumberDict[matches[0].Groups[1].Value];
                            var last = Int32.TryParse(matches[^1].Groups[1].Value, out int c2) ? c2 : stringToNumberDict[matches[^1].Groups[1].Value];
                            calibrationNumber += last + (first * 10);
                        }

                        result += calibrationNumber;
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