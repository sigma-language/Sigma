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

        public Lexer(string source)
        {
            this.Source = source;
            this.Pos = -1;
            this.LineNo = 1;
            this.Col = 0;

            this.NextChar();
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

        char Peek()
        {
            if (this.Pos + 1 >= this.Source.Length)
                return '\0';

            return this.Source[this.Pos + 1];
        }

        public Token[] Tokenize()
        {
            List<Token> tokens = new();
            var token = this.GetToken();
            while (token.Kind != TokenType.EOF)
            {
                tokens.Add(token);
                this.NextChar(length: token.Length);
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
                ('+', _)   => CreateToken('+',  TokenType.PLUS),
                ('-', _)   => CreateToken('-',  TokenType.MINUS),
                ('/', _)   => CreateToken('/',  TokenType.SLASH),
                ('*', _)   => CreateToken('*',  TokenType.ASTERISK),
                ('=', '=') => CreateToken("==", TokenType.EQEQ),
                ('=', _)   => CreateToken('=',  TokenType.EQ),

                ('\0', _)  => CreateToken('\0', TokenType.EOF),
                _ => null
            };

            if (token is null)
                this.Abort($"Unknown character `{CurChar}` (U+{(int)CurChar}) at {LineNo}:{Col}");

            return token;
        }

        void Abort(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
            Environment.Exit(1);
        }
    }
}
