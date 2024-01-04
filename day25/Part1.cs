using System.Diagnostics;
using System.Text;

namespace day25
{
    public class Part1
    {
        public static int Result()
        {
            int result = 0;

            var graph = new Graph();

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
                            var key = kv[0];
                            var value = kv[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            foreach (var v in value)
                            {
                                graph.AddEdge(key, v, 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            var sw = new Stopwatch();
            sw.Start();

            var theGraph = (Graph)graph.Clone();
            var numVerticesInPartition = StoerWagner(theGraph);
            result = (graph.Vertices().Count - numVerticesInPartition) * numVerticesInPartition;

            sw.Stop();
            Console.WriteLine($"Time elapsed: {sw.Elapsed}");

            return result;
        }

        private class Graph : ICloneable
        {
            public Dictionary<(string S, string T), int> Edges { get; init; }

            public Graph() => Edges = [];

            public HashSet<string> Vertices() => new HashSet<string>(Edges.SelectMany(e => new[] { e.Key.S, e.Key.T }));

            public void AddEdge(string s, string t, int w = 1)
            {
                if (Edges.TryGetValue((s, t), out int W))
                {
                    Edges[(s, t)] = W + w;
                }
                else if (Edges.TryGetValue((t, s), out W))
                {
                    Edges[(t, s)] = W + w;
                }
                else
                {
                    Edges.Add((s, t), w);
                }
            }

            public void RemoveEdge(string s, string t)
            {
                Edges.Remove((s, t));
                Edges.Remove((t, s));
            }

            public void MergeVertices(string TT, string SS)
            {
                foreach (var edge in Edges.Where(e => e.Key.S == TT || e.Key.T == TT).ToList())
                {
                    if (edge.Key.S == SS || edge.Key.T == SS)
                    {
                        RemoveEdge(SS, TT);
                        continue;
                    }

                    (string S, string T) = edge.Key.S == TT ? (SS, edge.Key.T) : (edge.Key.S, SS);
                    AddEdge(S, T, edge.Value);
                    RemoveEdge(edge.Key.S, edge.Key.T);
                    RemoveEdge(SS, TT);
                }
            }

            public object Clone() => new Graph { Edges = Edges.ToDictionary(entry => entry.Key, entry => entry.Value) };

            public override string ToString() => string.Join(";", Edges.Select(e => $"{e.Key.S} -- {{ {e.Key.T} }}"));
        }

        // Ref:
        // https://en.wikipedia.org/wiki/Stoer%E2%80%93Wagner_algorithm
        // https://www.youtube.com/watch?v=AtkEpr7dsW4&t=162s
        private static int StoerWagner(Graph graph)
        {
            var start = graph.Vertices().First();
            (int W, string S, string T) Cut = (int.MaxValue, "", "");
            var merges = new List<(string S, string T)>();
            var phase = 0;
            var bestPhase = 0;

            while (graph.Vertices().Count > 1)
            {
                var C = StoerWagnerMinPhaseCut(start, (Graph)graph.Clone());

                if (C.W < Cut.W)
                {
                    Cut = C;
                    bestPhase = phase;
                }
                merges.Add((C.S, C.T));
                graph.MergeVertices(C.T, C.S);
                phase++;
            }

            // create a graph of just merged pieces. 
            // This graph will contain all the nodes in on complete partition connected
            // Note this is not the original graph. It could contain only vertices
            // from the partition we are interested in or from both paritions.
            // what is gaurentee that it will contain all vertices from at least one
            // parition and that the best/min cut will give the starting postion (S)
            // for all the nodes in that parition.
            var gg = new Graph();
            foreach (var (S, T) in merges.Take(bestPhase))
            {
                gg.AddEdge(S, T);
            }

            // Use bfs to count the nodes in the full partition from graph created above.
            // Use the S from the best cut.  T represents the other paritition.
            // Start search from S and it will find all the nodes in the full partition detected
            var partition = new HashSet<string>();
            var bfs = new Queue<string>();
            bfs.Enqueue(merges[bestPhase].T);
            while (bfs.Count > 0)
            {
                var vertex = bfs.Dequeue();
                if (partition.Contains(vertex)) continue;
                partition.Add(vertex);
                foreach (var edge in gg.Edges.Where(e => e.Key.S == vertex || e.Key.T == vertex))
                {
                    bfs.Enqueue(edge.Key.S);
                    bfs.Enqueue(edge.Key.T);
                }
            }

            return partition.Count;
        }

        private static (int W, string S, string T) StoerWagnerMinPhaseCut(string start, Graph graph)
        {
            var A = new List<string> { start };
            var V = graph.Vertices();
            (int W, string S, string T) Cut = (0, "", "");

            while (!V.All(A.Contains))
            {
                var edge = graph.Edges
                    .Where(e => e.Key.S == start || e.Key.T == start)
                    .MaxBy(e => e.Value);
                var v = edge.Key.T == start ? edge.Key.S : edge.Key.T;
                A.Add(v);

                if (graph.Edges.Count > 1) graph.MergeVertices(v, start);
            }

            Cut = (graph.Edges.First().Value, A[^2], A[^1]);

            return Cut;
        }

        public class MaxHeap : IComparer<int>
        {
            public int Compare(int a, int b) => b.CompareTo(a);
        }
    }
}
