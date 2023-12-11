namespace day10
{
    public class MazeRunner(int row, int col)
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;

        // Records have automatic destructing but with class we have to manually implement
        public void Deconstruct(out int Row, out int Col)
        {
            Row = this.Row;
            Col = this.Col;
        }

        public override string ToString()
        {
            return $"{Row},{Col}";
        }
    }
}