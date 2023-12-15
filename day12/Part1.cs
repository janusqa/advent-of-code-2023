using System.Runtime.CompilerServices;
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

            Console.WriteLine(IsValid("#.#.######".ToCharArray().ToList(), [1, 1, 3]));
            Console.WriteLine(IsValid(".#...#....###.".ToCharArray().ToList(), [1, 1, 3]));
            Console.WriteLine(IsValid(".#.###.#.######".ToCharArray().ToList(), [1, 3, 1, 6]));
            Console.WriteLine(IsValid("####.#...#...".ToCharArray().ToList(), [4, 1, 1]));
            Console.WriteLine(IsValid("#....######..#####.".ToCharArray().ToList(), [1, 6, 5]));
            Console.WriteLine(IsValid(".###.##....#".ToCharArray().ToList(), [3, 2, 1]));

            return result;
        }

        public static bool IsValid(List<char> springs, List<int> blocks)
        {
            int pointer = 0;
            int blockLength = 0;
            int blocksProcessed = 0;

            foreach (var spring in springs)
            {
                if (spring == '.')
                {
                    if (pointer < blocks.Count && blockLength == blocks[pointer]) pointer++;
                    if (blockLength > 0) blocksProcessed++;
                    blockLength = 0;
                    continue;
                }
                blockLength++;
            }

            if (springs[^1] == '#')
            {
                if (blockLength > 0) blocksProcessed++;
            }

            return pointer == blocks.Count - 1 && pointer == blocksProcessed - 1;
        }
    }
}