using System.Text;

namespace day09
{
    class Part2
    {
        public static long Result()
        {
            long result = 0;

            var sequences = new List<List<long>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day09/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sequences.Add(line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var sequence in sequences)
            // {
            //     Console.WriteLine(string.Join(" ", sequence));
            // }

            foreach (var sequence in sequences)
            {
                result += PrevTerm(sequence);
            }

            return result;
        }

        private static long PrevTerm(List<long> sequence)
        {
            if (sequence.Aggregate(true, (acc, t) => acc && (t == 0))) return 0;

            var resultant = new List<long>();

            for (int i = 0; i < sequence.Count; i++)
            {
                if (i == 0) continue;
                resultant.Add(sequence[i] - sequence[i - 1]);
            }

            return sequence[0] - PrevTerm(resultant);
        }
    }
}