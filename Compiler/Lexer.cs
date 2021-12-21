using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Lexer
    {
        string Source { get; set; }
        char CurChar { get; set; }
        int Pos { get; set; }
        int LineNo { get; set; }
        int Col { get; set; }

        Dictionary<string, TokenType> _KEYWORDS;

        public Lexer(string source)
        {
            this.Source = source;
            this.Pos = -1;
            this.LineNo = 1;
            this.Col = 0;

            this.NextChar();

            this._KEYWORDS = new()
            {
                { "if", TokenType.IF },
            };
        }

        Token CreateToken(char text, TokenType kind)
            => new(text.ToString(), kind, this.LineNo, this.Col);
        Token CreateToken(string text, TokenType kind)
            => new(text, kind, this.LineNo, this.Col);

        void HandleComment()
        {
            int newlineLength;
            while (!this.IsNewline(out newlineLength))
                this.Advance();
            this.HandleNewline(newlineLength);
        }

        void HandleNewline(int newlineLength)
        {
            this.Advance(newlineLength);
            this.LineNo += 1;
            this.Col = 1;
        }

        bool IsComment()
            => this.CurChar == '/' && this.Peek() == '/';

        bool IsWhitespace()
            => this.CurChar == ' ' || this.CurChar == '\t';

        bool IsNewline(out int length)
        {
            var newlineLength = (this.CurChar, this.Peek()) switch
            {
                ('\r', '\n') => 2,
                ('\n', _) => 1,
                _ => 0
            };
            length = newlineLength;
            return newlineLength > 0;
        }

        bool IsKeyword(string tokenText, out TokenType keyword)
            => this._KEYWORDS.TryGetValue(tokenText, out keyword);

        void NextChar(int length = 1)
        {
            this.Advance(length);
            for (int whitespaceCount = 1; whitespaceCount > 0;)
            {
                whitespaceCount = 0;
                int newlineLength;
                while (this.IsNewline(out newlineLength))
                {
                    this.HandleNewline(newlineLength);
                    whitespaceCount += 1;
                }

                while (this.IsWhitespace())
                {
                    this.Advance();
                    whitespaceCount += 1;
                }

                while (this.IsComment())
                {
                    this.HandleComment();
                    whitespaceCount += 1;
                }
            }
        }

        void Advance(int length = 1)
        {
            this.Pos += length;
            this.Col += length;

            if (this.Pos >= this.Source.Length)
            {
                this.CurChar = '\0';
                return;
            }

            this.CurChar = this.Source[this.Pos];
        }

        char Peek(int peekLength = 1)
        {
            if (this.Pos + peekLength >= this.Source.Length)
                return '\0';

            return this.Source[this.Pos + peekLength];
        }

        public Token[] Tokenize()
        {
            List<Token> tokens = new();
            var token = this.GetToken();
            while (token.Kind != TokenType.EOF)
            {
                tokens.Add(token);
                token = this.GetToken();
            }

            // Add the EOF token.
            tokens.Add(token);

            return tokens.ToArray();
        }

        Token GetToken()
        {
            var token = (this.CurChar, this.Peek()) switch
            {
                // TODO: ADD ALL TOKENS!
                // Numbers, binary / hex
                ('+', _)   => this.CreateToken('+',  TokenType.PLUS),
                ('-', _)   => this.CreateToken('-',  TokenType.MINUS),
                ('/', _)   => this.CreateToken('/',  TokenType.SLASH),
                ('*', _)   => this.CreateToken('*',  TokenType.ASTERISK),
                ('=', '=') => this.CreateToken("==", TokenType.EQEQ),
                ('=', _)   => this.CreateToken('=',  TokenType.EQ),

                ('\0', _)  => this.CreateToken('\0', TokenType.EOF),
                _ => null
            };

            // Numbers
            if (char.IsDigit(this.CurChar))
            {
                var startPos = this.Pos;
                var startCol = this.Col;

                var newPos = 1;
                while (char.IsDigit(this.Peek(newPos)))
                    newPos += 1;

                // Decimal
                if (this.Peek(newPos) == '.')
                {
                    newPos += 1;

                    if (!char.IsDigit(this.Peek(newPos)))
                        this.Abort($"Illegal character in number `{CurChar}` (U+{(int)CurChar}) at {LineNo}:{Col}");

                    while (char.IsDigit(this.Peek(newPos)))
                        newPos += 1;
                }

                newPos += startPos;

                var tokenText = this.Source[startPos..newPos];
                token = new(tokenText, TokenType.NUMBER, this.LineNo, startCol);
            }

            // Identifiers & Keywords.
            if (char.IsLetter(this.CurChar))
            {
                var startPos = this.Pos;
                var startCol = this.Col;

                var newPos = 1;
                while (char.IsLetterOrDigit(this.Peek(newPos)))
                    newPos += 1;

                newPos += startPos;
                var tokenText = this.Source[startPos..newPos];

                TokenType keyword;
                if (this.IsKeyword(tokenText, out keyword))
                    token = new(tokenText, keyword, this.LineNo, startCol);
                else
                    token = new(tokenText, TokenType.IDENT, this.LineNo, startCol);
            }

            if (token is null)
                this.Abort($"Unknown character `{CurChar}` (U+{(int)CurChar}) at {LineNo}:{Col}");

            this.NextChar(length: token.Length);
            return token;
        }

        void Abort(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
            Environment.Exit(1);
        }
    }
}
