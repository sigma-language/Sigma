namespace Compiler
{
    using System;
    using System.Collections.Generic;

    public class Lexer
    {
        private static Dictionary<string, TokenType> keywords = new ()
        {
            { "if", TokenType.IF },
            { "print", TokenType.PRINT },
        };

        private readonly TextLogger logger;
        private readonly string source;

        private char curChar;
        private int pos;
        private int lineNo;
        private int col;

        public Lexer(string source, TextLogger logger)
        {
            this.logger = logger;
            this.source = source;
            this.pos = -1;
            this.lineNo = 1;
            this.col = 0;

            this.Advance();
        }

        private string CurLocation
        {
            get => $"{this.lineNo}:{this.col}";
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new ();

            var token = this.GetToken();
            while (token.Kind != TokenType.EOF)
            {
                tokens.Add(token);
                token = this.GetToken();
            }

            // Include the EOF Token in the token array.
            tokens.Add(token);

            return tokens;
        }

        private Token CreateToken(char text, TokenType kind)
            => new (text.ToString(), kind, this.lineNo, this.col);

        private Token CreateToken(string text, TokenType kind)
            => new (text, kind, this.lineNo, this.col);

        private void HandleComment()
        {
            int newlineLength;
            while (!this.IsNewline(out newlineLength))
            {
                this.Advance();
            }

            this.HandleNewline(newlineLength);
        }

        private void HandleNewline(int newlineLength)
        {
            this.Advance(newlineLength);
            this.lineNo += 1;
            this.col = 1;
        }

        private bool IsNewline(out int length)
        {
            var newlineLength = (this.curChar, this.Peek()) switch
            {
                ('\r', '\n') => 2,
                ('\n', _) => 1,
                _ => 0
            };

            length = newlineLength;
            return newlineLength > 0;
        }

        private void SkipWhitespace()
        {
            bool IsComment()
                => this.curChar == '/' && this.Peek() == '/';

            bool IsWhitespace()
                => this.curChar == ' ' || this.curChar == '\t';

            for (int whitespaceCount = 1; whitespaceCount > 0;)
            {
                whitespaceCount = 0;

                int newlineLength;
                while (this.IsNewline(out newlineLength))
                {
                    this.HandleNewline(newlineLength);
                    whitespaceCount += 1;
                }

                while (IsWhitespace())
                {
                    this.Advance();
                    whitespaceCount += 1;
                }

                while (IsComment())
                {
                    this.HandleComment();
                    whitespaceCount += 1;
                }
            }
        }

        private void Advance(int length = 1)
        {
            this.pos += length;
            this.col += length;

            if (this.pos >= this.source.Length)
            {
                this.curChar = '\0';
                return;
            }

            this.curChar = this.source[this.pos];
        }

        private char Peek(int peekLength = 1)
        {
            if (this.pos + peekLength >= this.source.Length)
            {
                return '\0';
            }

            return this.source[this.pos + peekLength];
        }

        private Token LexBinNumber()
        {
            var startPos = this.pos;
            var startCol = this.col;

            this.Advance(2);
            if (!char.IsDigit(this.curChar))
            {
                var invalidNumber = this.source[startPos..(this.pos + 1)];
                this.logger.Abort($"Invalid binary literal: `{invalidNumber}` at {this.CurLocation}");
            }

            while (char.IsDigit(this.curChar))
            {
                if (!(this.curChar == '0' || this.curChar == '1'))
                {
                    this.logger.Abort($"Invalid digit in binary literal: `{this.curChar}` (U+{(int)this.curChar}) at {this.CurLocation}");
                }

                this.Advance();
            }

            var tokenText = this.source[startPos..this.pos];
            return new (tokenText, TokenType.BIN_NUMBER, this.lineNo, startCol);
        }

        private Token LexHexNumber()
        {
            bool IsHexChar(char character)
                => "0123456789abcdefABCDEF".Contains(character);

            var startPos = this.pos;
            var startCol = this.col;

            this.Advance(2);

            if (!IsHexChar(this.curChar))
            {
                var invalidNumber = this.source[startPos..(this.pos + 1)];
                this.logger.Abort($"Invalid hexadecimal literal: `{invalidNumber}` at {this.CurLocation}");
            }

            while (IsHexChar(this.curChar))
            {
                if (!IsHexChar(this.curChar))
                {
                    this.logger.Abort($"Invalid character in hexadecimal literal: `{this.curChar}` (U+{(int)this.curChar}) at {this.CurLocation}");
                }

                this.Advance();
            }

            var tokenText = this.source[startPos..this.pos];
            return new (tokenText, TokenType.HEX_NUMBER, this.lineNo, startCol);
        }

        private Token LexOctalNumber()
        {
            bool IsOctalChar(char character)
                => "01234567".Contains(character);

            var startPos = this.pos;
            var startCol = this.col;

            this.Advance(2);

            if (!IsOctalChar(this.curChar))
            {
                var invalidNumber = this.source[startPos..(this.pos + 1)];
                this.logger.Abort($"Invalid octal literal: `{invalidNumber}` at {this.CurLocation}");
            }

            while (IsOctalChar(this.curChar))
            {
                if (!IsOctalChar(this.curChar))
                {
                    this.logger.Abort($"Invalid character in octal literal: `{this.curChar}` (U+{(int)this.curChar}) at {this.CurLocation}");
                }

                this.Advance();
            }

            var tokenText = this.source[startPos..this.pos];
            return new (tokenText, TokenType.OCT_NUMBER, this.lineNo, startCol);
        }

        private Token LexNumber()
        {
            var startPos = this.pos;
            var startCol = this.col;

            while (char.IsDigit(this.curChar))
            {
                this.Advance();
            }

            // Decimal
            if (this.curChar == '.')
            {
                this.Advance();

                if (!char.IsDigit(this.curChar))
                {
                    this.logger.Abort($"Expected a digit after the decimal point, got: `{this.curChar}` (U+{(int)this.curChar}) at {this.CurLocation}");
                }

                while (char.IsDigit(this.curChar))
                {
                    this.Advance();
                }
            }

            var tokenText = this.source[startPos..this.pos];
            return new (tokenText, TokenType.NUMBER, this.lineNo, startCol);
        }

        private Token LexIdentifier()
        {
            bool IsIdentChar(char character)
                => char.IsLetterOrDigit(character) || character == '_';

            bool IsKeyword(string tokenText, out TokenType keyword)
                => Lexer.keywords.TryGetValue(tokenText, out keyword);

            var startPos = this.pos;
            var startCol = this.col;

            while (IsIdentChar(this.curChar))
            {
                this.Advance();
            }

            var tokenText = this.source[startPos..this.pos];

            TokenType keyword;
            if (IsKeyword(tokenText, out keyword))
            {
                return new (tokenText, keyword, this.lineNo, startCol);
            }

            return new (tokenText, TokenType.IDENT, this.lineNo, startCol);
        }

        private Token GetToken()
        {
            this.SkipWhitespace();
            var token = (this.curChar, this.Peek()) switch
            {
                #pragma warning disable SA1025 // CodeMustNotContainMultipleWhitespaceInARow
                // Delimiters
                (';', _)   => this.CreateToken(';', TokenType.SEMICOLON),
                ('(', _)   => this.CreateToken('(', TokenType.LPAREN),
                (')', _)   => this.CreateToken(')', TokenType.RPAREN),
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
                ('!', _)   => this.CreateToken('!', TokenType.BANG),

                // Bitwise Logical Operators
                ('&', _)   => this.CreateToken('&', TokenType.AND),
                ('~', _)   => this.CreateToken('~', TokenType.TILDE),
                ('|', _)   => this.CreateToken('|', TokenType.PIPE),
                ('^', _)   => this.CreateToken('^', TokenType.CARET),

                // Comparisons (1 long)
                ('>', _)   => this.CreateToken('>', TokenType.GT),
                ('<', _)   => this.CreateToken('<', TokenType.LT),
                ('?', _)   => this.CreateToken('?', TokenType.QUESTION),

                // Mathematical Operators
                ('+', _)   => this.CreateToken('+',  TokenType.PLUS),
                ('-', _)   => this.CreateToken('-',  TokenType.MINUS),
                ('/', _)   => this.CreateToken('/',  TokenType.SLASH),
                ('*', _)   => this.CreateToken('*',  TokenType.ASTERISK),
                ('%', _)   => this.CreateToken('%', TokenType.PERCENT),

                // Yes
                ('=', _)   => this.CreateToken('=', TokenType.EQ),
                ('"', _)   => this.CreateToken('"', TokenType.QUOTE),

                ('\0', _)  => this.CreateToken('\0', TokenType.EOF),
                _ => null
                #pragma warning restore SA1025 // CodeMustNotContainMultipleWhitespaceInARow
            };

            if (token is not null)
            {
                this.Advance(length: token.Length);
                return token;
            }

            // Binary Numbers.
            if (this.curChar == '0' && this.Peek() == 'b')
            {
                token = this.LexBinNumber();
            }

            // Hex Numbers.
            else if (this.curChar == '0' && this.Peek() == 'x')
            {
                token = this.LexHexNumber();
            }

            // Octal Numbers.
            else if (this.curChar == '0' && this.Peek() == 'o')
            {
                token = this.LexOctalNumber();
            }

            // Numbers
            else if (char.IsDigit(this.curChar))
            {
                token = this.LexNumber();
            }

            // Identifiers & Keywords.
            // Identifiers can start with either a letter or a "_".
            else if (char.IsLetter(this.curChar) || this.curChar == '_')
            {
                token = this.LexIdentifier();
            }

            if (token is null)
            {
               this.logger.Abort($"Unknown character `{this.curChar}` (U+{(int)this.curChar}) at {this.CurLocation}");
            }

            return token;
        }
    }
}
