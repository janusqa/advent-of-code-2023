using System.Text;

namespace day14
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;

            var platform = new List<List<char>>();
            var directions = new Dictionary<char, (int R, int C)> {
                    { 'N', (-1, 0)}, { 'W', (0, -1)}, { 'S', (1, 0)}, { 'E', (0, 1)}
                };

            try
            {
                using (StreamReader reader = new StreamReader(@"./day14/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        platform.Add([.. line.ToCharArray()]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var row in platform)
            // {
            //     Console.WriteLine(string.Join("", row));
            // }

            Tilt(platform, directions, 'N');

            result = Load(platform);
            return result;
        }

        public static void Tilt(List<List<char>> platform, Dictionary<char, (int R, int C)> directions, char direction)
        {
            var aspect = direction == 'N' || direction == 'S' ? "vert" : "horz";
            var dir = direction == 'N' || direction == 'S' ? -directions[direction].R : -directions[direction].C;

            // memoize the latest positions where stones can more to (or not)
            var landings = new Dictionary<int, (int fiber, char val)>();
            var firstFiber = direction == 'N' ? platform[0].Select((v, i) => (v, i)) :
                                                direction == 'W' ? platform.Select((v, i) => (v[0], i)) :
                                                direction == 'S' ? platform[^1].Select((v, i) => (v, i)) :
                                                platform.Select((v, i) => (v[^1], i));

            int fib = direction == 'N' || direction == 'W' ? 0 : direction == 'S' ? platform.Count - 1 : platform[0].Count - 1;
            foreach (var (val, index) in firstFiber)
            {
                landings[index] = (fib, val);
            }

            // Console.WriteLine(string.Join(" || ", landings.Select(l => $"({l.Value.fiber},{l.Key}): {l.Value.val}")));

            int rStart = aspect == "vert" ? direction == 'N' ? 1 : platform.Count - 2 : direction == 'E' ? platform[0].Count - 2 : 1;
            int rEnd = aspect == "vert" ? direction == 'N' ? platform.Count : 0 : direction == 'E' ? 0 : platform[0].Count;
            int cStart = aspect == "vert" ? direction == 'N' ? 0 : 0 : direction == 'E' ? 0 : 0;
            int cEnd = aspect == "vert" ? direction == 'N' ? platform[0].Count : platform[0].Count : direction == 'E' ? platform.Count : platform.Count;

            int i = rStart;
            while ((direction == 'N' || direction == 'W') ? i < rEnd : i >= rEnd)
            {
                int j = cStart;
                while (j < cEnd)
                {
                    int r = aspect == "vert" ? i : j;
                    int c = aspect == "vert" ? j : i;
                    int landing = aspect == "vert" ? c : r;
                    int fiber = aspect == "vert" ? r : c;

                    if (platform[r][c] == 'O' && landings[landing].val == '.')
                    {
                        landings[landing] = (landings[landing].fiber, platform[r][c]);
                        platform[r][c] = '.';
                        platform[aspect == "vert" ? landings[landing].fiber : r][aspect == "vert" ? c : landings[landing].fiber] = landings[landing].val;
                        landings[landing] = (landings[landing].fiber + dir, platform[r][c]);
                    }
                    else
                    {
                        if (platform[r][c] == 'O' || platform[r][c] == '#') landings[landing] = (fiber, platform[r][c]);
                        if (platform[r][c] == '.' && landings[landing].val != '.') landings[landing] = (fiber, platform[r][c]);
                    }
                    j++;
                }
                if (direction == 'N' || direction == 'W')
                    i++;
                else
                    i--;
            }
        }

        public static int Load(List<List<char>> platform)
        {
            int result = 0;
            for (int row = 0; row < platform.Count; row++)
            {
                result += platform[row].Where(r => r == 'O').Count() * (platform.Count - 1 - row + 1);
            }

            return result;
        }
    }
}