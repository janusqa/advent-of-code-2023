using System.Text;

namespace day04
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;

            try
            {
                using (StreamReader reader = new StreamReader(@"./day04/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] numbers = line.Split(": ")[1].Split(" | ");
                        var winningNumbers = numbers[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(number => Int32.Parse(number));
                        var hasNumbers = numbers[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(number => Int32.Parse(number));
                        var winnings = winningNumbers.Intersect(hasNumbers.Select(number => number)).ToArray<int>();
                        if (winnings.Length > 0)
                        {
                            result += (int)Math.Pow(2, winnings.Length - 1);
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