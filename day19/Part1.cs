using System.Data;
using System.Text;

namespace day19
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;
            var parts = new Queue<(Dictionary<char, int> P, string A)>();
            var workflows = new Dictionary<string, List<((char C, char O, int R) RL, string A)>>();

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

                    while ((line = reader.ReadLine()) != null)
                    {
                        var categories = line[1..(line.Length - 1)].Split(",", StringSplitOptions.RemoveEmptyEntries);
                        var part = new Dictionary<char, int>();
                        foreach (var category in categories)
                        {
                            part.Add(category[0], int.Parse(category[2..]));
                        }
                        parts.Enqueue((part, "in"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            while (parts.Count > 0)
            {
                var part = parts.Dequeue();
                result += Process(part, workflows, parts);
            }

            return result;
        }

        private static int Process(
           (Dictionary<char, int> P, string A) part,
           Dictionary<string, List<((char C, char O, int R) RL, string A)>> workflows,
           Queue<(Dictionary<char, int> P, string A)> parts
        )
        {
            int result = 0;

            if (part.A == "A") return part.P.Select(categories => categories.Value).Sum();
            if (part.A == "R") return 0;

            if (workflows.TryGetValue(part.A, out List<((char C, char O, int R) RL, string A)>? rules))
            {
                foreach (var rule in rules)
                {
                    var execute = true;
                    if (rule.RL != ('\0', '\0', 0))
                    {
                        execute = rule.RL.O == '>' ? part.P[rule.RL.C] > rule.RL.R : part.P[rule.RL.C] < rule.RL.R;
                    }
                    if (execute)
                    {
                        parts.Enqueue((part.P, rule.A));
                        break;
                    }
                }
            }

            return result;
        }
    }
}