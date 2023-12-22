using System.Text;

namespace day12
{
    public class Part2
    {
        public static long Result()
        {
            long result = 0;

            var recordings = new Dictionary<int, (List<char> S, List<int> B)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day12/input.txt", Encoding.UTF8))
                {
                    string? line;
                    int row = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sections = line.Split(" ");

                        var template = string.Join("?", Enumerable.Repeat(sections[0], 5));
                        var blocks = string.Join(",", Enumerable.Repeat(sections[1], 5));
                        recordings.Add(row, (template.ToCharArray().ToList(), blocks.Split(",").Select(int.Parse).ToList()));
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

            foreach (var recording in recordings)
            {
                // Console.WriteLine($"== {string.Join("", recording.Value.S)} [{string.Join(",", recording.Value.B)}] ==");
                result += Arrangements(recording.Value.S, recording.Value.B, []);
            }

            return result;
        }

        public static long Arrangements(List<char> template, List<int> blocks, Dictionary<string, long> memo)
        {
            string memoKey = new StringBuilder(string.Join("", template)).Append(string.Join("", blocks)).ToString();

            if (template.Count == 0) return blocks.Count > 0 ? 0 : 1;
            if (memo.TryGetValue(memoKey, out long cachedResult)) return cachedResult;

            // Console.WriteLine($"{depth - 1} -> {string.Join("", template)} {string.Join("", blocks)}");

            long result = 0;
            var symbols = new char[2] { '.', '#' };

            var first = template[0];
            var rest = template[1..];

            if (first == symbols[0])
            {
                // encoutered '.'
                result = Arrangements(rest, blocks, memo);
            }
            else if (first == '?')
            {
                // encountered '?'
                foreach (var symbol in symbols)
                {
                    result += Arrangements(new List<char>([symbol, .. rest]), blocks, memo);
                }
            }
            else
            {
                // encountered a '#'
                if (blocks.Count == 0)
                {
                    result = 0;
                }
                else
                {
                    // if the current block we have started to look at is less or equal to size of string AND
                    // any chararactes in the size of the block are only valid characters ('#', '?') then proceed
                    // else this is not a valid result
                    if (blocks[0] <= template.Count && template[..blocks[0]].All(s => s == symbols[1] || s == '?'))
                    {
                        if (blocks[0] == template.Count)
                        {
                            // if the size of the string remaing is the size of the block
                            // AND there this is the last block we are examining
                            // then this is a good result otherwise it's not a valid result
                            result = blocks.Count > 1 ? 0 : 1;
                        }
                        else if (template[blocks[0]] == symbols[0])
                        {
                            result = Arrangements(template[blocks[0]..], blocks[1..], memo);
                        }
                        else if (template[blocks[0]] == '?')
                        {
                            result = Arrangements(['.', .. template[(blocks[0] + 1)..]], blocks[1..], memo);
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }

            memo[memoKey] = result;
            return result;
        }
    }
}