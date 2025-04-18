using System.Data;
using System.Windows;

namespace Repl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            while (true)
            {
              
                Console.WriteLine(parser.E());
            }
        }
    }

    internal class Parser
    {
        private string? input;
        private int lookahead;
        private int line;
        public Parser() {
            line = 0;
        }
        private void FindNonBlankChar()
        {
            lookahead = Console.Read();
            while (lookahead == ' ' || lookahead == '\t')
            {
                lookahead = Console.Read();
            }
        }

        public void Parse(string input) {
            if (string.IsNullOrEmpty(input)) {
                return;
            }
        }

        private void Match(int t)
        {
            if (lookahead == t)
            {
                FindNonBlankChar();
                return;
            }

            throw new Exception("Erro de sintaxe");
        }

        public int E()
        {
            return R(N());
        }

        private int N()
        {
            bool isNumber = false;
            int num = 0;
            while (char.IsDigit((char)lookahead))
            {
                isNumber = true;
                num *= 10;
                num += lookahead ^ '0';
                Match(lookahead);
            }

            if (!isNumber) throw new Exception("Numero inválido");

            return num;
        }

        private int R(int n)
        {
            switch (lookahead)
            {
                case '+':
                    Match('+');
                    return n + E();
                case '-':
                    Match('-');
                    return n - E();
                case '*':
                    Match('*');
                    return n * E();
                case '/':
                    Match('/');
                    return n / E();
                case '\r':
                    return n;
            }
            throw new Exception("Expression error");
        }
    }
}
