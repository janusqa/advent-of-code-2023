using System.Text;

namespace day04
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;
            var matches = new Dictionary<int, int>();
            var cards = new Stack<int>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day04/input.txt", Encoding.UTF8))
                {
                    string? line;
                    int card = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        card++;
                        string[] numbers = line.Split(": ")[1].Split(" | ");
                        var winningNumbers = numbers[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(number => Int32.Parse(number));
                        var hasNumbers = numbers[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(number => Int32.Parse(number));
                        var winnings = winningNumbers.Intersect(hasNumbers.Select(number => number)).ToArray<int>();

                        matches.Add(card, winnings.Length);
                        cards.Push(card);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            while (cards.Count > 0)
            {
                result++;
                int card = cards.Pop();
                if (matches[card] > 0)
                {
                    for (int i = 1; i <= matches[card]; i++)
                    {
                        cards.Push(card + i);
                    }
                }
            }

            return result;
        }
    }
}