using System.Collections;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    parser.ResetLexer();
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    parser.ResetLexer();
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
                    string lexema = token is Word word ? word.lexema : throw new Exception("Syntax error");    
                    Match(Tag.ID);
                    switch (token.tag)
                    {
                        case '=':
                        Match('=');
                        int valor = E();
                        Env.Put(word.lexema, new Symbol(Tag.NUM, valor));
                        break;
                        case -1:
                        Console.WriteLine(I(lexema));
                        return;
                        default:
                        Console.WriteLine(R(I(lexema)));
                        return;
                        }
                    return;
                case Tag.NUM:
                    var number = token is Num num ? num.number : throw new Exception("Syntax error");
                    Match(Tag.NUM);
                    Console.WriteLine(R(number));
                    return;
                case -1:
                    return;
                case '(':
                    Console.WriteLine(E());
                    return;
            }

            throw new Exception("Syntax error");
        }

        public int I(string lexema) {
            return (int)(Env.Get(lexema) ?? throw new Exception("Variable " + lexema + " not found")).Value;
        }

        private int E()
        {
            return R(V());
        }

        private int R(int number)
        {
            switch(token.tag)
            {
                case '+':
                    Match('+');
                    return number + F();
                case '-':
                    Match('-');
                    return number - F();
                case -1:
                    return number;
                default:
                    return F(number);
            }
        }
        private int F()
        {
            switch (token.tag)
            {
                case '*':
                    Match('*');
                    return V() * F();
                case '/':
                    Match('/');
                    return V() / F();
                default:
                    return V();
            }
        }

        private int F(int number) 
        {
            switch(token.tag) 
            {
                case '*':
                    Match('*');
                    return number * V();
                case '/':
                    Match('/');
                    return number / V();
            }
            throw new Exception("Syntax error");
        }

        private int V() { 
            switch(token.tag) 
            {
                case Tag.NUM:
                    int number = token is Num num ? num.number : throw new Exception("Expetected number");
                    Match(Tag.NUM);
                    return R(number);
                case Tag.ID:
                    string lexema = token is Word word ? word.lexema : throw new Exception("Expected ID");
                    Match(Tag.ID);
                    return R(I(lexema));
                default:
                    return R(V());
            }
        }

        public void ResetLexer() {
            Lexer.setPos(0);
        }
    }
    internal class Lexer 
    {
        private Hashtable tokens;
        private string line;
        private int pos;

        public void SetLine(string line) { this.line = line; }
        public void setPos(int pos) { this.pos = pos; }
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

            while (pos < line.Length && (line[pos] == ' ' || line[pos] == '\t' || line[pos] == '\n') )
            {
                pos++;
            }

            if (pos == line.Length)
            {
                pos = 0;
                return new Token(-1);
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

                string s = b.ToString();
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
