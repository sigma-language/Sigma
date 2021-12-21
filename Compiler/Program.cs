using System;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //string source = "";
            //try
            //{
            //    source = System.IO.File.ReadAllText("test.sigma");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            // string source = @"+-=== a if";
            string source = @"+-123 asdwa   9.8654 if */";

            Lexer lexer = new(source);
            var tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                Console.WriteLine($"\"{token.Text}\" {token.Kind} at {token.LineNo}:{token.Col} ({token.Length})");
            }
        }
    }
}
