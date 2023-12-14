using System.Text;

namespace day13
{
    public class Part2
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

            using (StreamWriter writer = new StreamWriter(@"./day13/output.txt"))
            {
                foreach (var (pattern, key) in patterns.Select(p => (p.Value, p.Key)))
                {
                    writer.WriteLine($"Pattern: {key}");
                    bool reflection = false;
                    foreach (var (orientations, iOrientation) in pattern.Select((o, i) => (o, i)))
                    {
                        for (int line = 1; line < orientations.Count; line++)
                        {
                            int smuged = 0;
                            if (IsSmuged(orientations[line - 1], orientations[line]) || orientations[line - 1] == orientations[line])
                            {
                                int l1 = line - 1;
                                int l2 = line;
                                while (0 <= l1 && l2 < orientations.Count && (IsSmuged(orientations[l1], orientations[l2]) || orientations[l1] == orientations[l2]))
                                {
                                    // Console.WriteLine($"{(iOrientation == 0 ? "Row" : "Col")}: {l1}-{line - 1}, {line}-{l2}");
                                    if (IsSmuged(orientations[l1], orientations[l2])) smuged++;
                                    l1--; l2++;
                                }
                                if ((l1 + 1 == 0 || l2 - 1 == orientations.Count - 1) && (IsSmuged(orientations[l1 + 1], orientations[l2 - 1]) || orientations[l1 + 1] == orientations[l2 - 1]))
                                {
                                    if (smuged == 1)
                                    {
                                        foreach (var l in orientations)
                                        {
                                            writer.WriteLine(l);
                                        }
                                        writer.WriteLine($"{(iOrientation == 0 ? "Row" : "Col")} Matches: {l1 + 1}-{line - 1}, {line}-{l2 - 1} -> ({line - 1 - 0 + 1}: {(iOrientation == 0 ? (line - 1 - 0 + 1) * 100 : (line - 1 - 0 + 1))})");
                                        writer.WriteLine();
                                        result += iOrientation == 0 ? (line - 1 - 0 + 1) * 100 : (line - 1 - 0 + 1);
                                        reflection = true;
                                    };

                                }
                            }
                            if (reflection) break;
                        }
                        if (reflection) break;
                    }
                    // Console.WriteLine();
                }
            }

            return result;
        }

        public static bool IsSmuged(string l1, string l2)
        {
            int differences = 0;
            foreach (var (c1, c2) in l1.ToCharArray().Zip(l2.ToCharArray()))
            {
                if (c1 != c2) differences++;
            }

            // Console.WriteLine($"{l1} {l2} -> {differences}");

            return differences == 1;
        }
    }
}