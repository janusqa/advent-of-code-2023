using System.Text;

namespace day16
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;

            var contraption = new List<List<char>>();
            var beams = new Queue<((int R, int C) D, (int R, int C) L)>();
            var energized = new Dictionary<(int R, int C), int>();
            var visited = new List<((int R, int C) D, (int R, int C) L)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day16/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        contraption.Add([.. line.ToCharArray()]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var row in contraption)
            // {
            //     Console.WriteLine(string.Join("", row));
            // }

            for (int r = 0; r < contraption.Count; r++)
            {
                for (int c = 0; c < contraption[0].Count; c++)
                {
                    if (r == 0 || r == contraption.Count - 1)
                    {
                        energized.Clear();
                        visited.Clear();
                        beams.Clear();
                        var b = r == 0 ? ((1, 0), (r, c)) : ((-1, 0), (r, c));
                        beams.Enqueue(b);
                        while (beams.Count > 0)
                        {
                            var beam = beams.Dequeue();
                            if (visited.Contains(beam)) continue; // cycle detected
                            if (0 > beam.L.R || beam.L.R > contraption.Count - 1 || 0 > beam.L.C || beam.L.C > contraption[0].Count - 1) continue; // beam escaped from grid
                            visited.Add(beam);
                            if (energized.TryGetValue((beam.L.R, beam.L.C), out int seen))
                            {
                                energized[(beam.L.R, beam.L.C)] = seen + 1;
                            }
                            else
                            {
                                energized.Add((beam.L.R, beam.L.C), 1);
                            }
                            Encounter(beam, beams, contraption);
                        }
                        result = Math.Max(energized.Count, result);
                    }

                    if (c == 0 || c == contraption[0].Count - 1)
                    {
                        energized.Clear();
                        visited.Clear();
                        beams.Clear();
                        var b = c == 0 ? ((0, 1), (r, c)) : ((0, -1), (r, c));
                        beams.Enqueue(b);
                        while (beams.Count > 0)
                        {
                            var beam = beams.Dequeue();
                            if (visited.Contains(beam)) continue; // cycle detected
                            if (0 > beam.L.R || beam.L.R > contraption.Count - 1 || 0 > beam.L.C || beam.L.C > contraption[0].Count - 1) continue; // beam escaped from grid
                            visited.Add(beam);
                            if (energized.TryGetValue((beam.L.R, beam.L.C), out int seen))
                            {
                                energized[(beam.L.R, beam.L.C)] = seen + 1;
                            }
                            else
                            {
                                energized.Add((beam.L.R, beam.L.C), 1);
                            }
                            Encounter(beam, beams, contraption);
                        }
                        result = Math.Max(energized.Count, result);
                    }
                }
            }

            return result;
        }

        public static void Encounter(
            ((int R, int C) D, (int R, int C) L) beam,
            Queue<((int R, int C) D, (int R, int C) L)> beams,
            List<List<char>> contraption
        )
        {
            var direction = new Dictionary<char, (int R, int C)> {
                {'U', (-1,0)},
                {'R', (0,1)},
                {'D', (1,0)},
                {'L', (0,-1)}
            };

            if (contraption[beam.L.R][beam.L.C] == '.')
            {
                beams.Enqueue(((beam.D.R, beam.D.C), (beam.L.R + beam.D.R, beam.L.C + beam.D.C)));
            }
            else if (contraption[beam.L.R][beam.L.C] == '/')
            {
                if (beam.D == direction['U'])
                {
                    beams.Enqueue((direction['R'], (beam.L.R + direction['R'].R, beam.L.C + direction['R'].C)));
                }
                else if (beam.D == direction['R'])
                {
                    beams.Enqueue((direction['U'], (beam.L.R + direction['U'].R, beam.L.C + direction['U'].C)));
                }
                else if (beam.D == direction['D'])
                {
                    beams.Enqueue((direction['L'], (beam.L.R + direction['L'].R, beam.L.C + direction['L'].C)));
                }
                else if (beam.D == direction['L'])
                {
                    beams.Enqueue((direction['D'], (beam.L.R + direction['D'].R, beam.L.C + direction['D'].C)));
                }
            }
            else if (contraption[beam.L.R][beam.L.C] == '\\')
            {
                if (beam.D == direction['U'])
                {
                    beams.Enqueue((direction['L'], (beam.L.R + direction['L'].R, beam.L.C + direction['L'].C)));
                }
                else if (beam.D == direction['R'])
                {
                    beams.Enqueue((direction['D'], (beam.L.R + direction['D'].R, beam.L.C + direction['D'].C)));
                }
                else if (beam.D == direction['D'])
                {
                    beams.Enqueue((direction['R'], (beam.L.R + direction['R'].R, beam.L.C + direction['R'].C)));
                }
                else if (beam.D == direction['L'])
                {
                    beams.Enqueue((direction['U'], (beam.L.R + direction['U'].R, beam.L.C + direction['U'].C)));
                }
            }
            else if (contraption[beam.L.R][beam.L.C] == '|')
            {
                if (beam.D == direction['U'])
                {
                    beams.Enqueue((direction['U'], (beam.L.R + direction['U'].R, beam.L.C + direction['U'].C)));
                }
                else if (beam.D == direction['L'] || beam.D == direction['R'])
                {
                    beams.Enqueue((direction['U'], (beam.L.R + direction['U'].R, beam.L.C + direction['U'].C)));
                    beams.Enqueue((direction['D'], (beam.L.R + direction['D'].R, beam.L.C + direction['D'].C)));
                }
                else if (beam.D == direction['D'])
                {
                    beams.Enqueue((direction['D'], (beam.L.R + direction['D'].R, beam.L.C + direction['D'].C)));
                }
            }
            else if (contraption[beam.L.R][beam.L.C] == '-')
            {
                if (beam.D == direction['U'] || beam.D == direction['D'])
                {
                    beams.Enqueue((direction['R'], (beam.L.R + direction['R'].R, beam.L.C + direction['R'].C)));
                    beams.Enqueue((direction['L'], (beam.L.R + direction['L'].R, beam.L.C + direction['L'].C)));
                }
                else if (beam.D == direction['R'])
                {
                    beams.Enqueue((direction['R'], (beam.L.R + direction['R'].R, beam.L.C + direction['R'].C)));
                }
                else if (beam.D == direction['L'])
                {
                    beams.Enqueue((direction['L'], (beam.L.R + direction['L'].R, beam.L.C + direction['L'].C)));
                }
            }
        }
    }
}