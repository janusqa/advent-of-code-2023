using Microsoft.Z3;
using System.Text;

namespace day24
{
    public class Part2
    {
        public static double Result()
        {
            double result = 0;

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

            // using Microsovt.z3 solover to solve a system of equations
            var z3Ctx = new Context();
            var z3Solver = z3Ctx.MkSolver();

            // our rock
            var x = z3Ctx.MkIntConst("x");
            var y = z3Ctx.MkIntConst("y");
            var z = z3Ctx.MkIntConst("z");
            var vx = z3Ctx.MkIntConst("vx");
            var vy = z3Ctx.MkIntConst("vy");
            var vz = z3Ctx.MkIntConst("vz");

            foreach (var (hailStone, i) in hail.Select((s, i) => (s, i)))
            {
                if (i > 2) break;
                var t = z3Ctx.MkRealConst($"t{i}");

                // hailstone
                var hx = z3Ctx.MkInt(Convert.ToInt64(hailStone.X));
                var hy = z3Ctx.MkInt(Convert.ToInt64(hailStone.Y));
                var hz = z3Ctx.MkInt(Convert.ToInt64(hailStone.Z));
                var hvx = z3Ctx.MkInt(Convert.ToInt64(hailStone.Vx));
                var hvy = z3Ctx.MkInt(Convert.ToInt64(hailStone.Vy));
                var hvz = z3Ctx.MkInt(Convert.ToInt64(hailStone.Vz));

                // create system of equations based on above
                //
                // Left side of equations
                var xLS = z3Ctx.MkAdd(x, z3Ctx.MkMul(t, vx)); // x = x0 + t * vx)
                var yLS = z3Ctx.MkAdd(y, z3Ctx.MkMul(t, vy)); // y = y0 + t * vy)
                var zLS = z3Ctx.MkAdd(z, z3Ctx.MkMul(t, vz)); // z = z0 + t * vz)

                // Right side of equations
                var xRS = z3Ctx.MkAdd(hx, z3Ctx.MkMul(t, hvx)); // hx = hx0 + t * hvx)
                var yRS = z3Ctx.MkAdd(hy, z3Ctx.MkMul(t, hvy)); // hy = hy0 + t * hvy)
                var zRS = z3Ctx.MkAdd(hz, z3Ctx.MkMul(t, hvz)); // hz = hz0 + t * hvz)

                z3Solver.Add(t >= 0);
                z3Solver.Add(z3Ctx.MkEq(xLS, xRS));
                z3Solver.Add(z3Ctx.MkEq(yLS, yRS));
                z3Solver.Add(z3Ctx.MkEq(zLS, zRS));
            }

            z3Solver.Check();
            var z3Model = z3Solver.Model;

            var rx = z3Model.Eval(x);
            var ry = z3Model.Eval(y);
            var rz = z3Model.Eval(z);

            result = double.Parse(rx.ToString()) + double.Parse(ry.ToString()) + double.Parse(rz.ToString());

            return result;
        }

        private class HailStone((double x, double y, double z) position, (double vx, double vy, double vz) velocity)
        {
            public double X { get; set; } = position.x;
            public double Y { get; set; } = position.y;
            public double Z { get; set; } = position.z;
            public double Vx { get; set; } = velocity.vx;
            public double Vy { get; set; } = velocity.vy;
            public double Vz { get; set; } = velocity.vz;

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