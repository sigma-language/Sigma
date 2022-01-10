namespace Compiler
{
    public class Token
    {
        public Token(string text, TokenType kind, int lineno, int col, int? length = null)
        {
            this.Text = text;
            this.Length = length ?? text.Length;
            this.Kind = kind;
            this.LineNo = lineno;
            this.Col = col;
        }

        public string Text { get; }

        public int Length { get; }

        public TokenType Kind { get; }

        public int LineNo { get; }

        public int Col { get; }

        public string Location
        {
            get => $"{this.LineNo}:{this.Col}";
        }
    }
}
