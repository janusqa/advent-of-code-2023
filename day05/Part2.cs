using System.Text;

namespace day05
{
    class Part2
    {
        public static long Result()
        {
            long result = 0;
            List<(long S, long R)> sources = [];
            var maps = new Dictionary<string, List<(long D, long S, long R)>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day05/input.txt", Encoding.UTF8))
                {
                    string? line;
                    string currentMapKey = "";
                    List<long> seeds = [];
                    if ((line = reader.ReadLine()) != null)
                    {
                        seeds.AddRange(line.Split(": ", StringSplitOptions.RemoveEmptyEntries)[1]
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(Int64.Parse));
                    }
                    for (int i = 1; i < seeds.Count; i += 2)
                    {
                        sources.Add((seeds[i - 1], seeds[i]));
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

                    // Console.WriteLine(string.Join(", ", sources));
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

            List<(long S, long R)> sourcesMapped = [];
            foreach (var map in maps)
            {
                foreach (var (D, S, R) in map.Value)
                {
                    foreach (var source in sources.ToList())
                    {

                        if ((S > source.S + source.R - 1) || (source.S > S + R - 1)) continue;

                        long intersectStart = Math.Max(S, source.S);
                        long intersectEnd = Math.Min(S + R - 1, source.S + source.R - 1);
                        long intersectRange = intersectEnd - intersectStart + 1;
                        sourcesMapped.Add((D + (intersectStart - S), intersectRange));

                        if (intersectStart > source.S)
                            sources.Add((source.S, (intersectStart - 1) - source.S + 1));
                        if (intersectEnd < source.S + source.R - 1)
                            sources.Add((intersectEnd + 1, (source.S + source.R - 1) - (intersectEnd + 1) + 1));

                        sources.Remove(source);
                    }
                }
                sources.AddRange(sourcesMapped);
                sourcesMapped.Clear();
            }

            result = sources.Select(value => value.S).Min();

            return result;
        }
    }
}