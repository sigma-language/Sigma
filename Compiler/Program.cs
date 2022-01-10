#pragma warning disable SA1600
namespace Compiler
{
    using System;
    using Compiler.Visitors;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("UPDATE ALL MATCH STATEMENTS WITH CUSTO MESSAGE");
            // UPDATE ALL MATCH STATEMENTS WITH CUSTO MESSAGE
            string source = @"
                ->
                    if (1 == 1) ->
                        print 0o105;
                    <-
                    if (2 >= 0xFA2) ->
                        print 0b01001;
                    <-
                <-
            ";

            TextLogger logger = new (Console.Error);

            Lexer lexer = new (source, logger);
            var tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                Console.WriteLine($"\"{token.Text}\" {token.Kind} at {token.Location} ({token.Length})");
            }

            SigmaParser parser = new (tokens, logger);
            var result = parser.Parse();

            DotFormatGenerator dotter = new ();
            Console.WriteLine(dotter.GenerateDotString(result));
        }
    }
}
