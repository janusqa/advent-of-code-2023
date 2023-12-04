namespace day03
{
    public record Schematic(int NumRows, int NumCols, List<Number> Numbers, List<Symbol> Symbols)
    {

        public int RUBound
        {
            get => NumRows - 1;
            init { }
        }

        public int CUBound
        {
            get => NumCols - 1;
            init { }
        }
    }
    public record SchematicItem(int Start, int End, int Length, int Row);
    public record Number(int Value, int Start, int End, int Length, int Row) : SchematicItem(Start, End, Length, Row);
    public record Symbol(string Value, int Start, int End, int Length, int Row) : SchematicItem(Start, End, Length, Row);
    public class NumberComparer : IEqualityComparer<Number>
    {
        public bool Equals(Number? n1, Number? n2)
        {
            if (n1 == null && n2 == null) return true;
            if (n1 == null || n2 == null) return false;
            if (n1.Start == n2.Start && n1.End == n2.End && n1.Row == n2.Row) return true;
            return false;
        }

        public int GetHashCode(Number n)
        {
            string code = $"{n.Start},{n.End},{n.Row}";
            return code.GetHashCode();
        }

    }
}