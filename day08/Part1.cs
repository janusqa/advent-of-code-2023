using System.Text;
using System.Text.RegularExpressions;

namespace day08
{
    class Part1
    {
        public static int Result()
        {
            int result = 0;

            string directions = "";
            var nodes = new Dictionary<string, (string L, string R)>();
            var rx = new Regex(@"([A-Z]{3})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            try
            {
                using (StreamReader reader = new StreamReader(@"./day08/input.txt", Encoding.UTF8))
                {
                    string? line;

                    if ((line = reader.ReadLine()) != null)
                    {
                        directions = line;
                    }

                    while ((line = reader.ReadLine()) != null)
                    {
                        MatchCollection matches = rx.Matches(line);
                        if (matches.Count == 3)
                        {
                            nodes[matches[0].Value] = (matches[1].Value, matches[2].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Console.WriteLine(directions);
            // foreach (var node in nodes)
            // {
            //     Console.WriteLine($"{node.Key} = ({node.Value.L}, {node.Value.R})");
            // }

            string position = "AAA";
            int pointer = 0;

            while (position != "ZZZ")
            {
                position = directions[pointer] == 'L' ? nodes[position].L : nodes[position].R;
                // wrap pointer around to 0 using modulo. Note we increment pointer first then convert it
                pointer = (++pointer - 1 + directions.Length + 1) % directions.Length;
                result++;
            }

            return result;
        }


    }
}