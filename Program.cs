using System.Collections;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;
using System.Text;

namespace Repl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new();
            do
            {
                Console.Write(">>> ");
                try
                {
                    var str = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    parser.Parse(str);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);      
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            } while (true);
        }
    }

    internal class Parser
    {
        private Lexer Lexer;
        private Token token;
        private Env Env;

        public Parser() 
        {
            Lexer = new Lexer();
            Env = new Env(null);
        }

        public void Parse(string str)
        {
            Lexer.SetLine(str);
            token = Lexer.Scan();
            S();
        }

        private void Match(int tag)
        {
            if (token.tag != tag) throw new Exception("Syntax error");
            token = Lexer.Scan();
        }

        public void S() 
        {
            switch (token.tag)
            {
                case Tag.ID:
                    /*if (token is Word word)
                    {
                        Match(Tag.ID);
                        switch (token.tag)
                        {
                            case '=':
                                Match('=');
                                int valor = E();
                                Env.Put(word.lexema, new Symbol(Tag.NUM, valor));
                                break;
                            case -1:
                                Console.WriteLine(I(word.lexema).Value);
                                return;
                            default:
                                Console.WriteLine(E());
                                return;
                        }
                    } else
                    {
                        throw new Exception("Invalid token");
                    }
                        break;*/
                    
                    break;
                case Tag.NUM:
                    var number = token is Num num ? num.number : throw new Exception("Syntax error");
                    Match(Tag.NUM);
                    Console.WriteLine(R(number));
                    break;
                case -1:
                    break;
                default:
                    throw new Exception("Syntax error");
            }
        }

        private Symbol I(String lexema)
        {
            return Env.Get(lexema) ?? throw new Exception("Variable "+ lexema +  "not found");
        }

        private int E() {
            switch (token.tag) 
            {
                case '+':
                    return V() + F();
                case '-':
                    return V() - F();
                default:
                    F();
            }
        }

        private int V()
        {
            switch(token.tag) { 
                case Tag.ID:
                    var lexema = token is Word word ? word.lexema : throw new Exception("Not a var");
                    Match(Tag.ID);
                    return (int)I(lexema).Value;
                case Tag.NUM:
                    var number = token is Num num ? num.number : throw new Exception("Not a number");
                    Match(Tag.NUM);
                    return number;
                case '(':
                    Match('(');
                    var n = E();
                    Match(')');
                    return n;
                default:
                    return E();
            }
        }

    }
    internal class Lexer 
    {
        private Hashtable tokens;
        private string line;
        private int pos;

        public void SetLine(string line) { this.line = line; }
        public Lexer() {
            tokens = [];
            pos = 0;
            
        }

        public void Reserve(Word token) {  tokens.Add(token.lexema, token); }

        public void Remove(Word token) {  tokens.Remove(token.lexema); }

        public Token Scan() {
            if (pos == line.Length)
            {
                pos = 0;
                return new Token(-1);
            }

            while (line[pos] == ' ' || line[pos] == '\t' || line[pos] == '\n' )
            {
                pos++;
            }

            if (char.IsDigit(line[pos])) {
                int v = 0;
                do
                {
                    v = 10 * v + (line[pos] ^ '0');
                    pos++;
                } while (pos != line.Length && char.IsDigit(line[pos]));
                return new Num(v);
            }

            if (char.IsLetter(line[pos])) {
                StringBuilder b = new();

                do
                {
                    b.Append(line[pos]);
                    pos++;
                } while (pos != line.Length && char.IsLetter(line[pos]));

                String s = b.ToString();
                Word w = (Word)tokens[s];
                if (w != null) {
                    return w;
                }

                w = new Word(Tag.ID, s);
                tokens.Add(s, w);
                return w;
            }

            Token t = new(line[pos]);
            pos++;
            return t;
        }
    }
}
