using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repl
{
    internal class Token(int tag)
    {
        public readonly int tag = tag;
    }

    internal class Num(int number) : Token(Tag.NUM) {
        public readonly int number = number;
    }

    internal class Word(int t, string lexema) : Token(t) {
        public readonly string lexema = new(lexema);
    }

    internal class Tag {
        public const int NUM = 256;
        public const int ID = 257;
    }
}
