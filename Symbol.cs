using System;
using System.Collections;

namespace Repl
{
    internal class Symbol(int Tag, object Value)
    {
        public int Tag = Tag;
        public object Value = Value;

    }

    internal class Env(Env? prev)
    {
        private Hashtable table = [];
        protected Env? prev = prev;

        public void Put(string nome, Symbol symbol) 
        {
            if (table.ContainsKey(nome))
            {
                table[nome] = symbol;
                return;
            }

            table.Add(nome, symbol);
        }

        public Symbol? Get(string nome)
        {
            for (Env e = this; e != null; e = e.prev) {
                Symbol found = (Symbol)e.table[nome];
                if(found != null) return found;
            }
            return null;
        }
    }
}
