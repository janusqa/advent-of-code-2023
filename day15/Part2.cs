using System.Text;

namespace day15
{
    public class Part2
    {
        public static int Result()
        {
            int result = 0;
            string initilizationSequence = "";
            List<(string label, string focalLength)>[] boxes = new List<(string label, string focalLength)>[256];

            try
            {
                using (StreamReader reader = new StreamReader(@"./day15/input.txt", Encoding.UTF8))
                {
                    string? line = reader.ReadLine();
                    initilizationSequence = String.IsNullOrEmpty(line) ? "" : line;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var step in initilizationSequence.Split(",").ToList())
            {
                if (step.Contains('='))
                {
                    var lens = step.Split("=");
                    int box = Hash(lens[0]);
                    if (boxes[box] == null) boxes[box] = [];
                    int oldLensAt = boxes[box].FindIndex(l => l.label == lens[0]);
                    if (oldLensAt == -1)
                    {
                        boxes[box].Add((lens[0], lens[1]));
                    }
                    else
                    {
                        boxes[box].Insert(oldLensAt, (lens[0], lens[1]));
                        boxes[box].RemoveAt(oldLensAt + 1);
                    }
                }
                else
                {
                    var lens = step.Split("-");
                    int box = Hash(lens[0]);
                    boxes[box]?.RemoveAll(l => l.label == lens[0]);
                }
            }

            foreach (var (box, boxIndex) in boxes.Select((b, i) => (b, i)))
            {
                if (box == null || box.Count == 0) continue;
                foreach (var ((label, focalLength), lensIndex) in box.Select((l, i) => (l, i)))
                {
                    result += (1 + boxIndex) * (lensIndex + 1) * (Int32.Parse(focalLength));
                }
            }

            return result;
        }

        public static int Hash(string plainText)
        {
            int currentValue = 0;
            foreach (char c in plainText)
            {
                currentValue += (int)c;
                currentValue *= 17;
                currentValue %= 256;
            }

            return currentValue;
        }
    }
}