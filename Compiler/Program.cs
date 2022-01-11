#pragma warning disable SA1600
namespace Compiler
{
    using System;
    using Compiler.Visitors;

    public class Program
    {
        public static void Main(string[] args)
        {
            string source = @"
                ->
                    int sus = 1 + 1;
                    if (1) ->
                        print -5 * 4;
                        // print (5 * 4 - 2) / (4 < 3 ? 2 : 4);
                        // print (5 * 4 - 2) / (4 > 3 ? 2 : 4);
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

            CodeGenerator codeGen = new ();
            Console.WriteLine(codeGen.GenerateCode(result));
        }
    }
}
