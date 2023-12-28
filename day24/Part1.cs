using System.Text;

namespace day24
{
    public class Part1
    {
        public static long Result()
        {
            long result = 0;

            var hail = new List<HailStone>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day24/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null)
                        {
                            var data = line.Split("@", StringSplitOptions.RemoveEmptyEntries);
                            var position = data[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();
                            var velocity = data[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();
                            hail.Add(new HailStone((position[0], position[1], position[2]), (velocity[0], velocity[1], velocity[2])));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // foreach (var hailStone in hail)
            // {
            //     Console.WriteLine(hailStone);
            // }

            (long lbound, long ubound) testArea = (200000000000000, 400000000000000);
            foreach (var (hailStone, i) in hail.Select((s, i) => (s, i)))
            {
                for (int j = i + 1; j < hail.Count; j++)
                {
                    var intersection = hailStone.PathIntersect(hail[j]);

                    if (intersection != null &&
                        testArea.lbound <= intersection.Value.x && intersection.Value.x <= testArea.ubound &&
                        testArea.lbound <= intersection.Value.y && intersection.Value.y <= testArea.ubound &&
                        intersection.Value.t1 >= 0 && intersection.Value.t2 >= 0
                    )
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        private class HailStone((double x, double y, double z) position, (double vx, double vy, double vz) velocity)
        {
            private double X { get; set; } = position.x;
            private double Y { get; set; } = position.y;
            private double Z { get; set; } = position.z;
            private double Vx { get; set; } = velocity.vx;
            private double Vy { get; set; } = velocity.vy;
            private double Vz { get; set; } = velocity.vz;

            public (double x, double y, double t1, double t2)? PathIntersect(HailStone hs)
            {
                // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
                //
                // y = mx + c (general equation of a line)
                // m = dy / dx = Vy / Vx
                //
                // y1 = a * x + c
                // y2 = b * x + d
                // a = dy / dx = Vy1 / Vx1  (slope is ratio of change of y over x or in old time terms the rise,y over run, x)
                // b = dy / dx = Vy2 / Vx2  (slope is ratio of change of y over x or in old time terms the rise,y over run, x)
                // c = y1 - ax
                // d = y2 - bx
                // ax + c = bx + d (line 1 intersects line 2  when y1 = y2)
                // ax - bx = d - c
                // x(a - b) = d - c
                // x = (d - c) / (a - b)
                // y = ax + c = bx + d
                // now generally x = x0 + (t * dx) ... x0 is initial x, x is x after some time t, dx is change over time, in our case velocity
                // t1 = (x1 - x10) / dx1 (this is the time at which the first rock passed this point)
                // t2 = (x2 - x10) / dx2 (this is the time at which the second rock passed this same point)
                // NB we are not looking for a collision, we are looking for if these stones ever crossed paths.
                // that is line B can pass thru a point at time t1 and line B can pass thru the same point at time t2
                // so there PATHS intersect even though the rocks them selves traveled thru the same point at different times.

                var a = Vy / Vx;
                var c = Y - (a * X);
                var b = hs.Vy / hs.Vx;
                var d = hs.Y - (b * hs.X);
                if (a == b) return null;
                var x = (d - c) / (a - b);
                var y = (a * x) + c;
                var t1 = (x - X) / Vx;
                var t2 = (x - hs.X) / hs.Vx;
                return (x, y, t1, t2);
            }
            public override string ToString() => $"<HailStone ({X},{Y},{Z}) @ ({Vx},{Vy},{Vz})>";
        }
    }
}