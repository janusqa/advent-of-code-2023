
using System.Text;

namespace day07
{
    class Part1
    {
        public static int Result()
        {
            int result = 0;

            List<Hand> hands = [];

            try
            {
                using (StreamReader reader = new StreamReader(@"./day07/input.txt", Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var hand = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        hands.Add(new Hand(hand[0], Int32.Parse(hand[1])));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Console.WriteLine(string.Join("\n", hands.Select(h => h)));
            // Console.WriteLine(string.Join(" ", Card.Weights.Select(w => $"{w.Key}:{w.Value}")));

            hands.Sort(new HandComparer());
            for (int i = 0; i < hands.Count; i++)
            {
                result += (i + 1) * hands[i].Bid;
            }

            return result;
        }

        public record Hand(string Label, int Bid)
        {
            private readonly List<char> _cards = [.. Label.ToCharArray()];
            private readonly Dictionary<char, int> _type = Label.GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());
            public List<char> Cards
            {
                get => _cards;
                init { }
            }

            public int Type
            {
                get => HandType();
                init { }
            }

            private int HandType()
            {
                if (_type.Count == 1) return 7; // Five of a kind
                if (_type.Count == 2 && _type.Max(c => c.Value) == 4) return 6; // Four of a kind
                if (_type.Count == 2 && _type.Max(c => c.Value) == 3) return 5; // Full house
                if (_type.Count == 3 && _type.Max(c => c.Value) == 3) return 4; // Three of a kind
                if (_type.Count == 3 && _type.Max(c => c.Value) == 2) return 3; // Two pair
                if (_type.Count == 4) return 2; // One pair
                if (_type.Count == 5) return 1; // High card
                return 0;
            }

            public override string ToString()
            {
                return $"{Label} || {Bid} || {string.Join(", ", Cards)} || {Type}";
            }
        }

        public record Card()
        {
            private readonly static Dictionary<char, int> _weights = new Dictionary<char, int> {
                {'2', 1}, {'3', 2}, {'4', 3}, {'5', 4}, {'6', 5}, {'7', 6}, {'8', 7},
                {'9', 8}, {'T', 9}, {'J', 10}, {'Q', 11}, {'K', 12}, {'A', 13}
            };

            public static Dictionary<char, int> Weights
            {
                get => _weights;
            }
        }

        private class HandComparer : IComparer<Hand>
        {

            public int Compare(Hand? a, Hand? b)
            {
                if (a == null && b == null) return 0;
                if (a == null || b == null) return 0;
                if (a.Type > b.Type) return 1;
                if (a.Type < b.Type) return -1;
                foreach (var (ax, bx) in a.Cards.Zip(b.Cards, (ax, bx) => (ax, bx)))
                {
                    if (Card.Weights[ax] > Card.Weights[bx]) return 1;
                    if (Card.Weights[ax] < Card.Weights[bx]) return -1;
                }

                return 0;
            }

            public int GetHashCode(Hand h)
            {
                string code = h.Label;
                return code.GetHashCode();
            }
        }


    }
}