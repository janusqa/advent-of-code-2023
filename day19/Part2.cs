using System.Text;

namespace day19
{
    public class Part2
    {
        public static long Result()
        {
            long result = 0;
            var workflows = new Dictionary<string, List<((char C, char O, int R) RL, string A)>>();
            var partRanges = new Queue<(Dictionary<char, (int L, int U)> RA, string A)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day19/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != "")
                    {
                        if (line != null)
                        {
                            var workflow = line[..line.IndexOf('{')];
                            var ruleSet = line[(line.IndexOf('{') + 1)..(line.Length - 1)].Split(",", StringSplitOptions.RemoveEmptyEntries);
                            var rules = new List<((char C, char O, int R) RL, string A)>();
                            foreach (var rule in ruleSet)
                            {
                                var instructions = rule.Split(":", StringSplitOptions.RemoveEmptyEntries);
                                var category = instructions.Length > 1 ? instructions[0][0] : '\0';
                                var op = instructions.Length > 1 ? instructions[0][1] : '\0';
                                var rating = instructions.Length > 1 ? int.Parse(instructions[0][2..]) : 0;
                                rules.Add(((category, op, rating), instructions[^1]));
                            }
                            workflows[workflow] = rules;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            partRanges.Enqueue((new Dictionary<char, (int L, int U)> { { 'x', (1, 4000) }, { 'm', (1, 4000) }, { 'a', (1, 4000) }, { 's', (1, 4000) } }, "in"));

            while (partRanges.Count > 0)
            {
                var partRange = partRanges.Dequeue();
                result += Process(partRange, workflows, partRanges);
            }

            return result;
        }

        private static long Process(
            (Dictionary<char, (int L, int U)> RA, string A) partRange,
            Dictionary<string, List<((char C, char O, int R) RL, string A)>> workflows,
            Queue<(Dictionary<char, (int L, int U)> RA, string A)> partRanges
        )
        {
            // Console.;

            long result = 0;

            if (partRange.A == "A") return (long)(partRange.RA['x'].U - partRange.RA['x'].L + 1) * (partRange.RA['m'].U - partRange.RA['m'].L + 1) * (partRange.RA['a'].U - partRange.RA['a'].L + 1) * (partRange.RA['s'].U - partRange.RA['s'].L + 1);
            if (partRange.A == "R") return 0;

            if (workflows.TryGetValue(partRange.A, out List<((char C, char O, int R) RL, string A)>? rules))
            {
                foreach (var rule in rules)
                {
                    if (rule.RL != ('\0', '\0', 0))
                    {
                        int L = partRange.RA[rule.RL.C].L;
                        int U = partRange.RA[rule.RL.C].U;
                        (int L, int U) passRange = rule.RL.O == '>' ? (rule.RL.R + 1, U) : (L, rule.RL.R - 1);
                        (int L, int U) failRange = rule.RL.O == '>' ? (L, rule.RL.R) : (rule.RL.R, U);
                        var goodRange = new Dictionary<char, (int L, int U)> { { 'x', partRange.RA['x'] }, { 'm', partRange.RA['m'] }, { 'a', partRange.RA['a'] }, { 's', partRange.RA['s'] } };
                        goodRange[rule.RL.C] = passRange;
                        partRanges.Enqueue((goodRange, rule.A));
                        partRange.RA[rule.RL.C] = failRange;
                    }
                    else
                    {
                        partRanges.Enqueue((partRange.RA, rule.A));
                    }
                }
            }

            return result;
        }

    }
}