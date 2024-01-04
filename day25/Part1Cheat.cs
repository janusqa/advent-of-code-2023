using System.Text;

namespace day25
{
    public class Part1Cheat
    {
        public static int Result()
        {
            int result = 0;

            var components = new Dictionary<string, HashSet<string>>();

            try
            {
                using (StreamReader reader = new StreamReader(@"./day25/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != null)
                        {
                            var kv = line.Split(":", StringSplitOptions.RemoveEmptyEntries);
                            components.Add(kv[0], new HashSet<string>(kv[1].Split(" ", StringSplitOptions.RemoveEmptyEntries)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // STEP 1: Comment out everything under STEP 3
            // run the command below
            // run in termainal -> s "dotnet run --project day25"

            // STEP 3: Uncomment the code below, plug in the edges to be removed from STEP 2,  and run the application again
            RemoveEdge("bbp", "dvr", components);
            RemoveEdge("gtj", "tzj", components);
            RemoveEdge("jzv", "qvq", components);
            result = BFS("bbp", components) * BFS("dvr", components);

            // STEP 2: Generate visual graph using the generated .dot file
            using (StreamWriter writer = new StreamWriter(@"./day25/components.dot"))
            {
                writer.Write(ToDot(components));
            }
            // Run the command below to generate svg and visually inspect it for the three edges to discard
            // dot -v -Tsvg -Kneato -O [<filename>].dot
            // dot -v -Tsvg -Kneato -O ./day25/components.dot

            return result;
        }

        private static string ToDot(Dictionary<string, HashSet<string>> components)
        {
            StringBuilder graph = new StringBuilder();
            graph.Append(@"graph G {");
            graph.Append(Environment.NewLine);
            foreach (var component in components)
            {
                graph.Append('\t');
                graph.Append(component.Key);
                graph.Append(@" -- {");
                graph.Append(string.Join(", ", component.Value));
                graph.Append('}');
                graph.Append(Environment.NewLine);
            }
            graph.Append('}');
            graph.Append(Environment.NewLine);

            return graph.ToString();
        }

        private static void RemoveEdge(string node1, string node2, Dictionary<string, HashSet<string>> components)
        {
            components[node1]?.Remove(node2);
            components[node2]?.Remove(node1);
        }

        private static int BFS(string start, Dictionary<string, HashSet<string>> components)
        {
            var visited = new HashSet<string>();
            var gQueue = new Queue<string>();
            int gCount = 0;

            gQueue.Enqueue(start);

            while (gQueue.Count > 0)
            {
                var node = gQueue.Dequeue();

                if (visited.Contains(node)) continue;
                visited.Add(node);
                gCount++;

                foreach (var component in components)
                {
                    if (components.TryGetValue(node, out HashSet<string>? neighbors))
                    {
                        foreach (var n in neighbors)
                        {
                            gQueue.Enqueue(n);
                        }
                    }
                    if (component.Value.Contains(node)) gQueue.Enqueue(component.Key);
                }
            }

            return gCount;
        }
    }
}