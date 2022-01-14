namespace Compiler
{
    public enum TokenType
    {
        #pragma warning disable SA1602 // EnumerationItemsMustBeDocumented
        EOF,
        PLUS,
        MINUS,
        SLASH,
        ASTERISK,
        EQEQ,
        EQ,
        IDENT,
        IF,
        NUMBER,
        HEXNUMBER,
        BINNUMBER,
        SEMICOLON,
        BANG,
        TILDE,
        AND,
        GT,
        PERCENT,
        QUOTE,
        PIPE,
        QUESTION,
        CARET,
        LT,
        LPAREN,
        RPAREN,
        OPEN_ARROW,
        CLOSE_ARROW,
        NOTEQ,
        GTEQ,
        LTEQ,
        ANDAND,
        OROR,
        BIN_NUMBER,
        HEX_NUMBER,
        OCT_NUMBER,
        COLON,
        PRINT,

        // Types
        INT,
        FLOAT,
        BOOL,
        WHILE,
        SWITCH,
        BIG_ARROW,
        COMMA,
#pragma warning restore SA1602 // EnumerationItemsMustBeDocumented
    }

#pragma warning disable SA1649 // SA1649FileNameMustMatchTypeName
    public static class TokenTypeExtensions
#pragma warning restore SA1649 // SA1649FileNameMustMatchTypeName
    {
        public static bool IsBuiltinType(this TokenType kind)
            => kind == TokenType.INT || kind == TokenType.FLOAT || kind == TokenType.BOOL;
    }
}
