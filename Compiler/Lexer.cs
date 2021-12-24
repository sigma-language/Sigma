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

            this.Advance();

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

        bool IsIdentChar(char character)
            => char.IsLetterOrDigit(character) || character == '_';

        bool IsKeyword(string tokenText, out TokenType keyword)
            => this._KEYWORDS.TryGetValue(tokenText, out keyword);

        void SkipWhitespace()
        {
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

        Token LexNumber()
        {
            var startPos = this.Pos;
            var startCol = this.Col;

            while (char.IsDigit(this.CurChar))
                this.Advance();

            // Decimal
            if (this.CurChar == '.')
            {
                this.Advance();

                if (!char.IsDigit(this.CurChar))
                    this.Abort($"Illegal character in number `{CurChar}` (U+{(int)CurChar}) at {LineNo}:{Col}");

                while (char.IsDigit(this.CurChar))
                    this.Advance();
            }

            var tokenText = this.Source [startPos..this.Pos];
            return new(tokenText, TokenType.NUMBER, this.LineNo, startCol);
        }

        Token LexIdentifier()
        {
            var startPos = this.Pos;
            var startCol = this.Col;

            while (this.IsIdentChar(this.CurChar))
                this.Advance();

            var tokenText = this.Source[startPos..this.Pos];

            TokenType keyword;
            if (this.IsKeyword(tokenText, out keyword))
                return new(tokenText, keyword, this.LineNo, startCol);
            
            return new(tokenText, TokenType.IDENT, this.LineNo, startCol);
        }

        Token GetToken()
        {
            this.SkipWhitespace();
            var token = (this.CurChar, this.Peek()) switch
            {
                // Delimiters
                (';', _) => this.CreateToken(';', TokenType.SEMICOLON),
                ('(', _) => this.CreateToken('(', TokenType.LPAREN),
                (')', _) => this.CreateToken(')', TokenType.RPAREN),
                ('-', '>') => this.CreateToken("->", TokenType.OPEN_ARROW),
                ('<', '-') => this.CreateToken("<-", TokenType.CLOSE_ARROW),

                // Comparisons (2 long)
                ('=', '=') => this.CreateToken("==", TokenType.EQEQ),
                ('!', '=') => this.CreateToken("!=", TokenType.NOTEQ),
                ('>', '=') => this.CreateToken(">=", TokenType.GTEQ),
                ('<', '=') => this.CreateToken("<=", TokenType.LTEQ),

                // Logical Operators
                ('&', '&') => this.CreateToken("&&", TokenType.ANDAND),
                ('|', '|') => this.CreateToken("||", TokenType.OROR),
                ('!', _) => this.CreateToken('!', TokenType.BANG),

                // Bitwise Logical Operators
                ('&', _) => this.CreateToken('&', TokenType.AND),
                ('~', _) => this.CreateToken('~', TokenType.TILDE),
                ('|', _) => this.CreateToken('|', TokenType.PIPE),
                ('^', _) => this.CreateToken('^', TokenType.CARET),

                // Comparisons (1 long)
                ('>', _) => this.CreateToken('>', TokenType.GT),
                ('<', _) => this.CreateToken('<', TokenType.LT),
                ('?', _) => this.CreateToken('?', TokenType.QUESTION),
                
                // Mathematical Operators
                ('+', _)   => this.CreateToken('+',  TokenType.PLUS),
                ('-', _)   => this.CreateToken('-',  TokenType.MINUS),
                ('/', _)   => this.CreateToken('/',  TokenType.SLASH),
                ('*', _)   => this.CreateToken('*',  TokenType.ASTERISK),
                ('%', _) => this.CreateToken('%', TokenType.PERCENT),

                // Yes
                ('=', _) => this.CreateToken('=', TokenType.EQ),
                ('"', _) => this.CreateToken('"', TokenType.QUOTE),

                ('\0', _)  => this.CreateToken('\0', TokenType.EOF),
                _ => null
            };

            if (token is not null)
            {
                this.Advance(length: token.Length);
                return token;
            }

            // Numbers
            if (char.IsDigit(this.CurChar))
                token = LexNumber();

            // Identifiers & Keywords.
            // Identifiers can start with either a letter or a "_".
            if (char.IsLetter(this.CurChar) || this.CurChar == '_')
                token = this.LexIdentifier();

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
