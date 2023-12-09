using System.Text;
using System.Text.RegularExpressions;

namespace day08
{
    class Part2
    {
        public static int Result()
        {
            int result = 0;

            string directions = "";
            var nodes = new Dictionary<string, (string L, string R)>();
            var memo = new Dictionary<string, List<string>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day08/input.txt", Encoding.UTF8))
                {
                    string? line;
                    var rx = new Regex(@"([A-Z0-9]{3})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

            var positions = nodes.Where(node => node.Key.EndsWith('A')).Select(node => node.Key).ToList();
            int pointer = 0;

            using (StreamWriter writer = new StreamWriter(@"./day08/output.txt"))
            {
                while (!positions.Select(node => node[^1]).Aggregate(true, (acc, n) => acc && (n == 'Z')))
                {
                    writer.WriteLine($"{string.Join(" ", positions.Select(node => node[^1]))} -- {string.Join(" ", positions)}");

                    foreach (var position in positions.ToList())
                    {
                        positions.Remove(position);
                        positions.Add(directions[pointer] == 'L' ? nodes[position].L : nodes[position].R);
                    }
                    // wrap pointer around to 0 using modulo. Note we increment pointer first then convert it
                    pointer = (++pointer - 1 + directions.Length + 1) % directions.Length;
                    result++;
                }

                writer.WriteLine($"{string.Join(" ", positions.Select(node => node[^1]))} -- {string.Join(" ", positions)}");
            }

            return result;
        }
    }
}