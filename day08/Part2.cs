using System.Text;
using System.Text.RegularExpressions;

namespace day08
{
    class Part2
    {
        public static long Result()
        {
            long result = 0;

            string directions = "";
            var nodes = new Dictionary<string, (string L, string R)>();

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

            var locations = nodes.Where(node => node.Key.EndsWith('A')).Select(node => node.Key).ToArray();
            var ghostsMeta = new Dictionary<int, long[]>(); // long[] -> times seen, last seen, cycle length
            int pointer = 0;

            using (StreamWriter writer = new StreamWriter(@"./day08/output.txt"))
            {

                while (!locations.Select(node => node[^1]).Aggregate(true, (acc, n) => acc && (n == 'Z')))
                {
                    // writer.WriteLine($"{string.Join(" ", locations.Select(node => node[^1]))} -- {string.Join(" ", locations)}");

                    foreach (var location in locations.Select((label, index) => (label, index)))
                    {
                        locations[location.index] = directions[pointer] == 'L' ? nodes[location.label].L : nodes[location.label].R;

                        // each time we see a location ending with 'Z' collect the following metadata:
                        // 1. the times we see such a node
                        // 2. when last did we see the node a.k.a at which step did we last see node
                        // 3. calculate the number of steps it took since we last saw such a node to seeing it again.
                        if (location.label.EndsWith('Z'))
                        {
                            if (ghostsMeta.TryGetValue(location.index, out long[]? value))
                            {
                                value[0] += 1;
                                if (value[0] > 1)
                                {
                                    value[2] = result - value[1];
                                    value[1] = result;
                                }
                            }
                            else
                            {
                                ghostsMeta[location.index] = [1, result, 0]; // times seen, last seen, cycle length
                            }
                            writer.WriteLine(string.Join(" ", ghostsMeta.Select(x => $"{x.Key}: {string.Join(" ", x.Value.Select(x => x))}")));
                        }
                    }
                    if (ghostsMeta.Count == 6 && ghostsMeta.Aggregate(true, (acc, meta) => meta.Value[0] > 1))
                    {
                        // we have detected all cycles at which each "ghost" sees a node ending with 'Z'
                        // we can now calculate the number of steps for each "ghost" to finally arrive 
                        // simultaneously on a node ending with 'Z' by taking the LCM of the number of
                        // times it takes each "ghost" to see a node ending with 'Z'. Luckily 
                        // in this case it is a constant number of times for each ghost.
                        return LCM(ghostsMeta.Select(meta => meta.Value[2]).ToArray());
                    }
                    // wrap pointer around to 0 using modulo. Note we increment pointer first then convert it
                    pointer = (++pointer - 1 + directions.Length + 1) % directions.Length;
                    result++;
                }
            }

            return result;
        }

        private static long LCM(long[] numbers)
        {
            long lcmValue = 1;
            long divisor = 2;

            while (true)
            {

                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < numbers.Length; i++)
                {

                    // LCM (n1, n2, ... 0) = 0.
                    // For negative number we convert into
                    // positive and calculate lcmValue.
                    if (numbers[i] == 0)
                    {
                        return 0;
                    }
                    else if (numbers[i] < 0)
                    {
                        numbers[i] = numbers[i] * (-1);
                    }
                    if (numbers[i] == 1)
                    {
                        counter++;
                    }

                    // Divide numbers by devisor if complete
                    // division i.e. without remainder then replace
                    // number with quotient; used for find next factor
                    if (numbers[i] % divisor == 0)
                    {
                        divisible = true;
                        numbers[i] = numbers[i] / divisor;
                    }
                }

                // If divisor able to completely divide any number
                // from array multiply with lcmValue
                // and store into lcmValue and continue
                // to same divisor for next factor finding.
                // else increment divisor
                if (divisible)
                {
                    lcmValue *= divisor;
                }
                else
                {
                    divisor++;
                }

                // Check if all numbers is 1 indicate 
                // we found all factors and terminate while loop.
                if (counter == numbers.Length)
                {
                    return lcmValue;
                }
            }
        }
    }
}