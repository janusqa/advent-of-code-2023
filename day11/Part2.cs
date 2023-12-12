using System.Text;

namespace day11
{
    public class Part2
    {
        public static long Result()
        {
            long result = 0;
            var universe = new List<List<char>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day11/input.txt", Encoding.UTF8))
                {
                    string? line;
                    var colSums = new Dictionary<int, int>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        var row = new List<char>();
                        universe.Add(line.ToCharArray().ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // calculate shortest paths for prime universe
            var galaxies = new Dictionary<int, (int R, int C)>();
            MapGalaxies(universe, galaxies);
            var pairs = new List<List<int>>();
            Combinations(pairs, [], galaxies.Select(g => g.Key).ToList());
            // Console.WriteLine(string.Join(" | ", pairs.Where(p => p.Count == 2).Select(p => string.Join(", ", p))));
            long primeUniversShortestPaths = 0;
            foreach (var pair in pairs.Where(p => p.Count == 2))
            {
                primeUniversShortestPaths += Manhattan((galaxies[pair[0]].R, galaxies[pair[0]].C), (galaxies[pair[1]].R, galaxies[pair[1]].C));
            }

            // calulate shortest paths for expanded universe (x2). This is one expansion above prime universe
            var expandedUniverse = new List<List<char>>();
            var expandedGalaxies = new Dictionary<int, (int R, int C)>();
            Expand(universe, expandedUniverse, 2);
            MapGalaxies(expandedUniverse, expandedGalaxies);
            var expandedPairs = new List<List<int>>();
            Combinations(expandedPairs, [], expandedGalaxies.Select(g => g.Key).ToList());
            long expandedUniversShortestPaths = 0;
            foreach (var pair in expandedPairs.Where(p => p.Count == 2))
            {
                expandedUniversShortestPaths += Manhattan((expandedGalaxies[pair[0]].R, expandedGalaxies[pair[0]].C), (expandedGalaxies[pair[1]].R, expandedGalaxies[pair[1]].C));
            }

            // calculate shortest paths for any expanded universe
            // take the differnce in shortest paths beween two universes that have a distance expansion of (x2) 
            // i.e the expansion is just one step above. Multiply that by the expansion factor minus one that you need.
            // Finally add that result to the shortestpath from the prime universe.
            result = ((expandedUniversShortestPaths - primeUniversShortestPaths) * (1000000 - 1)) + primeUniversShortestPaths;

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

        public static void Expand(List<List<char>> universePrime, List<List<char>> universe, int factor)
        {
            var colSums = new Dictionary<int, int>();
            foreach (var row in universePrime)
            {
                foreach (var (v, col) in row.Select((v, col) => (v, col)))
                {
                    int value = v == '.' ? 0 : 1;
                    if (colSums.TryGetValue(col, out int colSum))
                    {
                        colSums[col] = colSum + value;
                    }
                    else
                    {
                        colSums.Add(col, value);
                    }
                }
                universe.Add([.. row]);
                // If we encouter a blank row add another blank row to expand the universe
                // Remember we need to do the same for blank columns. Will do this later.
                if (!row.Where(c => c == '#').Any())
                {
                    for (int i = 0; i < factor - 1; i++)
                    {
                        universe.Add([.. row]);
                    }
                }
            }

            // Add blank columns to universe to expand universe
            foreach (var row in universe)
            {
                foreach (var col in colSums.Reverse())
                {
                    if (col.Value == 0)
                    {
                        for (int i = 0; i < factor - 1; i++)
                        {
                            row.Insert(col.Key, '.');
                        }
                    }
                }
            }
        }

        public static void MapGalaxies(List<List<char>> universe, Dictionary<int, (int R, int C)> galaxies)
        {
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
        }
    }


}