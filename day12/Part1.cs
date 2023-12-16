using System.Text;

namespace day12
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;

            var recordings = new Dictionary<int, (List<char> A, List<int> B)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day12/input_test.txt", Encoding.UTF8))
                {
                    string? line;
                    int row = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sections = line.Split(" ");
                        recordings.Add(row, (sections[0].ToCharArray().ToList(), sections[1].Split(",").Select(int.Parse).ToList()));
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var row in recordings)
            {
                Console.WriteLine($"{row.Key} -> {string.Join("", row.Value.A)} | {string.Join(", ", row.Value.B)}");
            }

            Console.WriteLine("---");
            Combinations([], [], ['a', 'b', 'c']);

            return result;
        }

        public static bool IsValid(List<char> springs, List<int> blocks)
        {
            int pointer = 0;
            int blockLength = 0;
            int blocksProcessed = 0;

            foreach (var (spring, index) in springs.Select((s, i) => (s, i)))
            {
                if (spring == '.' || (spring == '#' && index == springs.Count - 1))
                {
                    if (spring == '#') blockLength++;
                    if (pointer < blocks.Count && blockLength == blocks[pointer]) pointer++;
                    if (blockLength > 0) blocksProcessed++;
                    blockLength = 0;
                    continue;
                }
                blockLength++;
            }

            return pointer == blocksProcessed;
        }

        public static void Combinations(List<List<char>> combinations, List<char> memo, List<char> source, int start = 0)
        {
            combinations.Add(new List<char>(memo));

            for (int i = start; i < source.Count; i++)
            {
                // choose the current char
                memo.Add(source[i]);

                // backtrack the new combination
                Combinations(combinations, memo, source, i + 1);

                // don't choose the char
                memo.RemoveAt(memo.Count - 1);
            }
        }
    }
}