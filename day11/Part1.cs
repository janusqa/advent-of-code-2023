using System.Text;

namespace day11
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            var universe = new List<List<char>>();
            var galaxies = new Dictionary<int, (int R, int C)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day11/input.txt", Encoding.UTF8))
                {
                    string? line;
                    var colSums = new Dictionary<int, int>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        var row = new List<char>();
                        foreach (var (v, col) in line.Select((v, col) => (v, col)))
                        {
                            int value = v == '.' ? 0 : 1;
                            row.Add(v);
                            if (colSums.TryGetValue(col, out int colSum))
                            {
                                colSums[col] = colSum + value;
                            }
                            else
                            {
                                colSums.Add(col, value);
                            }
                        }
                        universe.Add(row);
                        // If we encouter a blank row add another blank row to expand the universe
                        // Remember we need to do the same for blank columns. Will do this later.
                        if (!row.Where(c => c == '#').Any()) universe.Add([.. row]);
                    }

                    // Add blank columns to universe to expand universe
                    foreach (var row in universe)
                    {
                        foreach (var col in colSums.Reverse())
                        {
                            if (col.Value == 0) row.Insert(col.Key, '.');
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Tag galaxies and their locations
            int numGalaxies = 0;
            foreach (var (row, rindex) in universe.Select((row, index) => (row, index)))
            {
                foreach (var (col, cindex) in row.Select((col, cindex) => (col, cindex)))
                {
                    if (col == '#') galaxies.Add(++numGalaxies, (rindex, cindex));
                }

                // Console.WriteLine(string.Join("", row));
            }
            // Console.WriteLine(string.Join(" | ", galaxies.Select(g => $"{g.Key}: {g.Value}")));

            var pairs = new List<List<int>>();
            Combinations(pairs, [], galaxies.Select(g => g.Key).ToList());
            // Console.WriteLine(string.Join(" | ", pairs.Where(p => p.Count == 2).Select(p => string.Join(", ", p))));

            foreach (var pair in pairs.Where(p => p.Count == 2))
            {
                result += Manhattan((galaxies[pair[0]].R, galaxies[pair[0]].C), (galaxies[pair[1]].R, galaxies[pair[1]].C));
            }

            return result;
        }

        public static int Manhattan((int R, int C) a, (int R, int C) b)
        {
            return Math.Abs(a.R - b.R) + Math.Abs(a.C - b.C);
        }

        public static void Combinations(List<List<int>> combinations, List<int> memo, List<int> source, int start = 0)
        {
            if (memo.Count == 2) combinations.Add(new List<int>(memo)); // only add combonations of length 2 to result. Just interested in pairs.

            for (int i = start; i < source.Count; i++)
            {
                // choose the current number
                // NB: we are only interested in pairs (combinations of length 2. Ignore everything else)
                if (memo.Count == 2) continue;
                memo.Add(source[i]);

                // backtrack the new combination
                Combinations(combinations, memo, source, i + 1);

                // don't choose the number
                memo.RemoveAt(memo.Count - 1);
            }
        }
    }


}