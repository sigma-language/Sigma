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
                    int check = 10;
                    int sus = check switch ->
                        4 => 0,
                        sus => 1 + 2,
                        2 * 5 => -5,
                    <-;
                    print sus;
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

            SymbolResolver symbolResolver = new SymbolResolver(logger);
            var table = symbolResolver.GenerateSymbolTable(result);
            table.Dump();

            CodeGenerator codeGen = new (table, logger);
            Console.WriteLine(codeGen.GenerateCode(result));
        }
    }
}
