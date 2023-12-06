using System.Text;

namespace day05
{
    class Part1
    {
        public static long Result()
        {
            long result = 0;
            List<long> sources = [];
            var maps = new Dictionary<string, List<(long D, long S, long R)>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day05/input.txt", Encoding.UTF8))
                {
                    string? line;
                    string currentMapKey = "";
                    if ((line = reader.ReadLine()) != null)
                    {
                        sources.AddRange(line.Split(": ", StringSplitOptions.RemoveEmptyEntries)[1]
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(Int64.Parse));
                    }
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("-to-"))
                        {
                            currentMapKey = line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];
                            maps[currentMapKey] = [];
                        }
                        else if (line.Trim().Length > 0)
                        {
                            var mapLineItem = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                            maps[currentMapKey].Add((mapLineItem[0], mapLineItem[1], mapLineItem[2]));
                        }
                    }

                    // Console.WriteLine(string.Join(", ", seeds));
                    // Console.WriteLine();
                    // foreach (var map in maps)
                    // {
                    //     Console.WriteLine(map.Key);
                    //     map.Value.ForEach(mapLineItem => Console.WriteLine(mapLineItem));
                    //     Console.WriteLine();
                    // }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var map in maps)
            {
                foreach (var (value, index) in sources.Select((value, index) => (value, index)).ToList())
                {
                    foreach (var (D, S, R) in map.Value)
                    {
                        if (S <= value && value <= S + R - 1)
                        {
                            sources[index] = D + (value - S);
                            break;
                        }
                    }
                }
            }

            result = sources.Min();

            return result;
        }
    }
}