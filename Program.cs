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

                    break;
                case Tag.NUM:
                    var number = token is Num num ? num.number : throw new Exception("Syntax error");
                    Match(Tag.NUM);
                    Console.WriteLine(R(number));
                    break;
                case -1:
                    break;
                default:
                    Console.WriteLine(E());
                    break;
            }
        }

        public int I(string lexema) {
            return (int)(Env.Get(lexema) ?? throw new Exception("Syntax Error")).Value;
        }

        private int E()
        {
            return R(V());
        }

        private int R(int number)
        {
            switch (token.tag) {
                case '+':
                    Match('+');
                    return number + F();
                case '-':
                    Match('-');
                    return number - F();
                default:
                    return F(number);
            }
        }

        private int F(int number)
        {
            switch (token.tag)
            {
                case '*':
                    Match('*');
                    return number * V();
                case '/':
                    Match('/');
                    return number / V();
                case '(':
                    Match('(');
                    var expr_val = R(number);
                    Match(')');
                    return expr_val;
                case -1:
                case ')':
                    return number;
                default:
                    return V();
            }
        }

        private int F()
        {
            switch(token.tag)
            {
                case '*':
                    Match('*');
                    return V() * V();
                case '/':
                    Match('/');
                    return V() / V();
                default:
                    return V();
            }
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
                case '(':
                    Match('(');
                    var expr_val = E();
                    Match(')');
                    return expr_val;
                default:
                    return E();
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
