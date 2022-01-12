namespace Compiler.Symbols
{
    public class VariableSymbol : Symbol
    {
        public VariableSymbol(string name, string type)
        {
            // HasBeenUsed???
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; init; }

        public string Type { get; init; }
    }
}
