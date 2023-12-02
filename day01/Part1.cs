using System.Text;
using System.Text.RegularExpressions;

namespace day01
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            var rx = new Regex(@"\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            try
            {
                using (StreamReader reader = new StreamReader(@"./day01/input.txt", Encoding.UTF8))
                {
                    string? line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        MatchCollection matches = rx.Matches(line);
                        if (matches.Count > 0)
                        {
                            if (Int32.TryParse($"{matches[0].Value}{matches[^1].Value}", out int calibrationNumber))
                            {
                                result += calibrationNumber;
                            }
                        }
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