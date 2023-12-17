using System.Text;

namespace day12
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;

            var recordings = new Dictionary<int, (List<char> S, List<int> B)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day12/input_test.txt", Encoding.UTF8))
                {
                    string? line;
                    int row = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sections = line.Split(" ");
                        StringBuilder sb0 = new StringBuilder(sections[0]);
                        StringBuilder sb1 = new StringBuilder(sections[1]);
                        for (int i = 0; i < 4; i++)
                        {
                            sb0.Append('?');
                            sb0.Append(sections[0]);
                            sb1.Append(',');
                            sb1.Append(sections[1]);

                        }

                        recordings.Add(row, (sb0.ToString().ToCharArray().ToList(), sb1.ToString().Split(",").Select(int.Parse).ToList()));
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var row in recordings)
            // {
            //     Console.WriteLine($"{row.Key} -> {string.Join("", row.Value.S)} | {string.Join(", ", row.Value.B)}");
            // }

            // Console.WriteLine(IsValid([.. ".###........".ToCharArray()], [3, 2, 1]));

            foreach (var recording in recordings)
            {
                // Console.WriteLine($"== {string.Join("", recording.Value.S)} [{string.Join(",", recording.Value.B)}] ==");
                result += Arrangements(recording.Value.S, recording.Value.B, "", recording.Value.S.Where(p => p == '?').Count(), []);
            }

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

            return pointer == blocksProcessed && pointer == blocks.Count;
        }

        public static int Arrangements(List<char> template, List<int> block, string pattern, int placeHolderCount, HashSet<string> memo)
        {
            if (memo.Contains(pattern)) return 0;

            var symbol = new char[2] { '.', '#' };

            int result = 0;

            for (int i = 0; i < placeHolderCount; i++)
            {
                string s = new StringBuilder(pattern).Append(symbol[i % 2]).ToString();
                // string s = $"{pattern}{symbol[i % 2]}";
                if (s.Length < placeHolderCount) result += Arrangements(template, block, s, placeHolderCount, memo);
                if (s.Length < placeHolderCount) continue;
                if (s.Where(c => c == '#').Count() != block.Sum() - template.Where(c => c == '#').Count()) continue;
                // Console.WriteLine($"level: {level} - len: {s.Length} - {s} - {s.Where(c => c == '#').Count()} - {string.Join("", template)} - {block.Sum()} - {template.Where(c => c == '#').Count()} - {block.Sum() - template.Where(c => c == '#').Count()}");
                if (memo.Contains(s)) continue;
                memo.Add(s);
                int patternIdx = 0;
                var templateCopy = template.ToList();

                // Console.WriteLine($"seen: {string.Join(" | ", seen)}");
                // Console.WriteLine($"template: {string.Join("", templateCopy)}");

                foreach (var (p, idx) in template.Select((p, i) => (p, i)))
                {
                    if (p == '?')
                    {
                        templateCopy.Insert(idx, s[patternIdx]);
                        templateCopy.RemoveAt(idx + 1);
                        patternIdx++;
                    }
                }
                if (IsValid(templateCopy, block))
                {
                    result++;
                    // Console.WriteLine($"valid: {string.Join("", templateCopy)}");
                }
            }

            memo.Add(pattern);
            return result;
        }
    }
}