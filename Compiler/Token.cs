using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Token
    {
        public string Text { get; }
        public int Length { get; }
        public TokenType Kind { get; }
        public int LineNo { get; }
        public int Col { get; }

        public Token(string text, TokenType kind, int lineno, int col, int? length = null)
        {
            Text = text;
            this.Length = length ?? text.Length;
            Kind = kind;
            LineNo = lineno;
            Col = col;
        }
    }
}
