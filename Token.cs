using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repl
{
    public class Token
    {
        private int tag;

        public Token(int tag)
        {
            this.tag = tag;
        }
    }

    public class Num : Token {
        public Num() : base(Tag.NUM) {}
    }

    public class Tag {
        public static int NUM = 512;
        public static int WORD = 533;
    }
}
