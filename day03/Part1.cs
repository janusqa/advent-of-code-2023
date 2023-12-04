using System.Text;
using System.Text.RegularExpressions;

namespace day03
{
    public class Part1
    {
        public static int Result()
        {
            var directions = new List<(int Row, int Col)> {
                 ( -1, 0 ), ( -1, 1 ), ( 0, 1 ), ( 1, 1 ), ( 1, 0 ), ( 1, -1 ), ( 0, -1 ), ( -1, -1 )
            };
            var partNumbers = new HashSet<Number>(new NumberComparer());

            int result = 0;
            var rx = new Regex(@"(\d{1,}|[^.])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            try
            {
                int numCols = 0;
                int row = 0;
                var Numbers = new List<Number>();
                var Symbols = new List<Symbol>();
                using (StreamReader reader = new StreamReader(@"./day03/input.txt", Encoding.UTF8))
                {
                    string? line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        MatchCollection matches = rx.Matches(line);
                        if (row == 0) numCols = line.Length;
                        if (matches.Count > 0)
                        {
                            for (int i = 0; i < matches.Count; i++)
                            {
                                if (Int32.TryParse(matches[i].Value, out int number))
                                {
                                    Numbers.Add(new Number(number, matches[i].Index, LastPos(matches[i].Index, matches[i].Length), matches[i].Length, row));
                                }
                                else
                                {
                                    Symbols.Add(new Symbol(matches[i].Value, matches[i].Index, LastPos(matches[i].Index, matches[i].Length), matches[i].Length, row));
                                }
                            }
                        }
                        row++;
                    }
                }
                var schematic = new Schematic(row, numCols, Numbers, Symbols);

                foreach (Symbol symbol in schematic.Symbols)
                {
                    foreach ((int Row, int Col) in directions)
                    {
                        int rPos = symbol.Row + Row;
                        int cPos = symbol.Start + Col;
                        if (!OutOfBounds(rPos, cPos, schematic.RUBound, schematic.CUBound))
                        {
                            foreach (Number number in schematic.Numbers)
                            {
                                if (rPos == number.Row && (number.Start <= cPos && cPos <= number.End))
                                {
                                    partNumbers.Add(number);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (Number partNumber in partNumbers)
            {
                result += partNumber.Value;
            }

            return result;
        }

        private static int LastPos(int firstPos, int len)
        {
            return (firstPos + len) - 1;
        }

        private static bool OutOfBounds(int row, int col, int rowBound, int colBound)
        {
            return (0 > row || row > rowBound || 0 > col || col > colBound);
        }
    }
}

