using System.Text;

namespace day15
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            string initilizationSequence = "";

            try
            {
                using (StreamReader reader = new StreamReader(@"./day15/input.txt", Encoding.UTF8))
                {
                    string? line = reader.ReadLine();
                    initilizationSequence = String.IsNullOrEmpty(line) ? "" : line;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var step in initilizationSequence.Split(",").ToList())
            {
                result += Hash(step);
            }


            return result;
        }

        public static int Hash(string plainText)
        {
            int currentValue = 0;
            foreach (char c in plainText)
            {
                currentValue += (int)c;
                currentValue *= 17;
                currentValue %= 256;
            }

            return currentValue;
        }
    }
}