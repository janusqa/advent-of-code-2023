using System.Text;

namespace day13
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;

            var patterns = new Dictionary<int, List<string>[]>();


            try
            {
                using (StreamReader reader = new StreamReader(@"./day13/input.txt", Encoding.UTF8))
                {
                    string? line;
                    int iPattern = 0;
                    while ((line = reader.ReadLine()) != null)
                    {

                        if (line.Trim().Length != 0)
                        {
                            if (patterns.TryGetValue(iPattern, out List<string>[]? pattern))
                            {
                                pattern[0].Add(line);
                            }
                            else
                            {
                                patterns.Add(iPattern, [[], []]);
                                patterns[iPattern][0].Add(line);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < patterns[iPattern][0][0].Length; i++)
                            {
                                patterns[iPattern][1].Add(string.Join("", patterns[iPattern][0].Select(row => row[i])));
                            }
                            iPattern++;
                        }
                    }
                    for (int i = 0; i < patterns[iPattern][0][0].Length; i++)
                    {
                        patterns[iPattern][1].Add(string.Join("", patterns[iPattern][0].Select(row => row[i])));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var pattern in patterns)
            // {
            //     foreach (var lines in pattern.Value)
            //     {
            //         foreach (var line in lines)
            //         {
            //             Console.WriteLine(line);
            //         }
            //         Console.WriteLine();
            //     }
            //     Console.WriteLine();
            // }

            foreach (var (pattern, key) in patterns.Select(p => (p.Value, p.Key)))
            {
                // Console.WriteLine($"Pattern: {key}");
                bool reflection = false;
                foreach (var (orientations, iOrientation) in pattern.Select((o, i) => (o, i)))
                {
                    for (int line = 1; line < orientations.Count; line++)
                    {
                        if (orientations[line - 1] == orientations[line])
                        {
                            int l1 = line - 1;
                            int l2 = line;
                            while (0 <= l1 && l2 < orientations.Count && (orientations[l1] == orientations[l2]))
                            {
                                // Console.WriteLine($"{(iOrientation == 0 ? "Row" : "Col")}: {l1}-{line - 1}, {line}-{l2}");
                                l1--; l2++;
                            }
                            if ((l1 + 1 == 0 || l2 - 1 == orientations.Count - 1) && (orientations[l1 + 1] == orientations[l2 - 1]))
                            {
                                // Console.WriteLine($"{(iOrientation == 0 ? "Row" : "Col")} Matches: {l1 + 1}-{line - 1}, {line}-{l2 - 1}");

                                result += iOrientation == 0 ? (line - 1 - 0 + 1) * 100 : (line - 1 - 0 + 1);
                                reflection = true;
                            }
                        }
                        if (reflection) break;
                    }
                    if (reflection) break;
                }
                // Console.WriteLine();
            }

            return result;
        }
    }
}