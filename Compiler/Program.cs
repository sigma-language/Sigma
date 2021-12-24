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
            string source = @"if ->_sus ===<- -> _ = 1 + 1; <-";

            Lexer lexer = new(source);
            var tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                Console.WriteLine($"\"{token.Text}\" {token.Kind} at {token.LineNo}:{token.Col} ({token.Length})");
            }
        }
    }
}
