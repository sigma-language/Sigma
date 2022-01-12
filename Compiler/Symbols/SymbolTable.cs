namespace Compiler.Symbols
{
    using System.Collections.Generic;
    using Scope = System.Collections.Generic.Dictionary<string, Compiler.Symbols.Symbol>;

    public class SymbolTable
    {

        private readonly List<Scope> scopes;
        private int currentScope;

        public SymbolTable()
        {
            this.scopes = new ();
            this.currentScope = -1;
        }

        public Scope CurrentScope => this.scopes[this.currentScope];

        public void EnterScope()
        {
            this.scopes.Add(new ());
            this.currentScope += 1;
        }

        public void LeaveScope()
        {
            this.currentScope -= 1;
        }

        public bool InsertSymbol(string name, Symbol symbol)
        {
            // Perhaps start using Exceptions instead? SymbolAlreadyExistsException?
            if (this.CurrentScope.ContainsKey(name))
            {
                return false;
            }

            this.CurrentScope.Add(name, symbol);
            return true;
        }

        public bool LookupSymbol(string name, out Symbol symbol)
        {
            for (int scopeIdx = this.currentScope; scopeIdx >= 0; scopeIdx--)
            {
                if (this.scopes[scopeIdx].TryGetValue(name, out symbol))
                {
                    return true;
                }
            }

            symbol = null;
            return false;
        }

        public void Dump()
        {
            for (int scopeIdx = 0; scopeIdx <= this.scopes.Count - 1; scopeIdx++)
            {
                var scope = this.scopes[scopeIdx];
                System.Console.WriteLine($"----{scopeIdx}----");

                foreach (var kvp in scope)
                {
                    var symbol = (VariableSymbol)kvp.Value;
                    System.Console.WriteLine($"{symbol.Name}:{symbol.Type}");
                }
            }
        }
    }
}
