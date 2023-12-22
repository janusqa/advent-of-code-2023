using System.Text;

namespace day20
{
    public class Part1
    {
        public static long Result()
        {
            long result = 0;
            var modules = new Dictionary<string, Module>();
            var pulses = new Queue<(string F, int P, string T)>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day20/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null)
                        {
                            var moduleDescriptor = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                            var type = moduleDescriptor[0][0] > 96 && moduleDescriptor[0][0] < 123 ? '\0' : moduleDescriptor[0][0];
                            var name = moduleDescriptor[0][0] > 96 && moduleDescriptor[0][0] < 123 ? moduleDescriptor[0] : moduleDescriptor[0][1..];
                            var neighbours = moduleDescriptor[1].Split(", ", StringSplitOptions.RemoveEmptyEntries);
                            if (name == "broadcaster")
                            {
                                modules.Add(name, new Brodcaster { Name = name, Outputs = neighbours });
                            }
                            else if (type == '%')
                            {
                                modules.Add(name, new FlipFlop { Type = type, Name = name, Outputs = neighbours });
                            }
                            else if (type == '&')
                            {
                                modules.Add(name, new Conjunction { Type = type, Name = name, Outputs = neighbours });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Module.Modules = modules;

            // foreach (var module in modules)
            // {
            //     Console.WriteLine(module.Value);
            // }

            long lowPulses = 0;
            long highPulses = 0;
            for (int i = 0; i < 1000; ++i)
            {
                lowPulses++;
                pulses.Enqueue(("button", 0, "broadcaster"));
                while (pulses.Count > 0)
                {
                    var pulse = pulses.Dequeue();
                    var sentPulses = Process(pulse, pulses, modules);
                    lowPulses += sentPulses.L;
                    highPulses += sentPulses.H;
                }
            }

            result = lowPulses * highPulses;

            return result;
        }

        private static (int L, int H) Process((string F, int P, string T) pulse, Queue<(string F, int P, string T)> pulses, Dictionary<string, Module> modules)
        {
            int lowPulses = 0;
            int highPulses = 0;
            if (modules.TryGetValue(pulse.T, out Module? module))
            {
                foreach (var message in module.Send(pulse.P, pulse.F))
                {
                    pulses.Enqueue(message);
                    if (message.P == 1) highPulses++;
                    if (message.P == 0) lowPulses++;

                    // Console.WriteLine($"{module.Name} -{(message.P == 0 ? "Low" : "High")}-> {message.T}");
                }
            }

            return (lowPulses, highPulses);
        }

        private abstract class Module
        {
            public char? Type { get; init; }
            public required string[] Outputs { get; init; }
            public required string Name { get; init; }
            public static Dictionary<string, Module>? Modules { get; set; }

            public abstract List<(string F, int P, string T)> Send(int received, string from);

            public override string ToString()
            {
                return $"{Type}{Name} -> {string.Join(", ", Outputs)}";
            }

        }

        private class FlipFlop(int state = 0) : Module
        {
            private int State { get; set; } = state;

            public override List<(string F, int P, string T)> Send(int received, string from)
            {
                if (received == 0)
                {
                    State = State == 0 ? 1 : 0;
                    return Outputs.Select(n => (Name, State, n)).ToList();
                }
                return [];
            }

            public override string ToString()
            {
                return $"{base.ToString()} || {State}";
            }
        }

        private class Conjunction : Module
        {
            private Dictionary<string, int> State { get; init; } = [];
            private int? Inputs { get; set; }

            public override List<(string F, int P, string T)> Send(int received, string from)
            {
                int? inputsCount = Inputs ?? Modules?.Where(m => m.Value.Outputs.Contains(Name)).Count();
                State[from] = received;

                // Console.WriteLine($"--->{Name}: {inputsCount}, State: {State.Count} -> {string.Join(", ", State.Select(s => $"{s.Key}: {s.Value}"))}");

                if (inputsCount == State.Count && State.All(p => p.Value == 1))
                {
                    return Outputs.Select(n => (Name, 0, n)).ToList();
                }
                return Outputs.Select(n => (Name, 1, n)).ToList();
            }

            public override string ToString()
            {
                return $"{base.ToString()} || {string.Join(", ", State.Select(s => $"{s.Key}: {s.Value}"))}";
            }
        }

        private class Brodcaster : Module
        {
            public override List<(string F, int P, string T)> Send(int received, string from)
            {
                return Outputs.Select(n => (Name, received, n)).ToList();
            }
        }
    }
}