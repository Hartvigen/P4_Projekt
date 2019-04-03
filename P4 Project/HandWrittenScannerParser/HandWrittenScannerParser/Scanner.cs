using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandWrittenScannerParser
{
    class Token
    {
        private int kind;
        private string value;

        public Token(Kinds kind, string value)
        {
            this.kind = (int)kind;
            this.value = value;
        }
    }

    enum Kinds : int
    {
        Number,
        IDENT,
        LBrack,
        RBrack,
        Assign,
        RParen,
        LParen,
        Func
    }

    class Inputter
    {
        public int line = 1;
        public int col = 0;
        private IEnumerator<char> input;

        public Inputter(string path)
        {
            input = File.ReadAllText(path, Encoding.UTF8).GetEnumerator();
        }

        public void MoveNext()
        {
            input.MoveNext();
            col++;
            if (input.Current.ToString().Contains('\n'))
            {
                line++;
                input.MoveNext();
                col = 1;
            }
        }

        public char Current()
        {
            return input.Current;
        }


    }

    class Scanner
    {
        private Inputter input;
        private Queue<Token> Tokens = new Queue<Token>();
        Regex numberRegex = new Regex("[0-9.]");
        Regex IDENTRegex = new Regex("[A-Za-z_]");
        Regex WSREgex = new Regex("\\s, \r");

        public Scanner(string path)
        {
            input = new Inputter(path);
        }


        public Token Peek()
        {
            return Tokens.Peek();
        }

        public void NextToken()
        {
            input.MoveNext();
            Token tkn = null;

            while(WSREgex.IsMatch(input.Current().ToString()))
            {
                input.MoveNext();
            }

            if (numberRegex.IsMatch(input.Current().ToString()))
            {
                tkn = NumberToken();
            }
            else if (IDENTRegex.IsMatch(input.Current().ToString()))
            {
                tkn = IdentOrFuncToken();
            }
            else
            {
                switch (input.Current())
                {
                    case '[':
                        tkn = new Token(Kinds.LBrack, "[");
                        break;
                    case ']':
                        tkn = new Token(Kinds.RBrack, "[");
                        break;
                    case '=':
                        tkn = new Token(Kinds.Assign, "[");
                        break;
                    case '(':
                        tkn = new Token(Kinds.LParen, "[");
                        break;
                    case ')':
                        tkn = new Token(Kinds.RParen, "[");
                        break;

                    default:
                        throw new Exception("Error: Cannot recognize symbol.");
                }
            }

            Tokens.Enqueue(tkn);


        }

        public Token NumberToken()
        {
            StringBuilder sb = new StringBuilder();

            while (numberRegex.IsMatch(input.Current().ToString()))
            {
                sb.Append(input.Current());
            }
            if(sb.ToString().Count(f => f == '.') > 1)
                throw new Exception($"Error: Number ending at line {input.line}, column {input.col} contains multiple dots\n");
            return new Token(Kinds.Number, sb.ToString());
        }

        public Token IdentOrFuncToken()
        {
            Token tkn;
            bool continuous = true;
            string value = "";

            while (continuous)
            {
                if (value.Contains("func "))//TODO check for space
                {
                    return tkn = new Token(Kinds.Func, "func");
                }

                if (IDENTRegex.IsMatch(input.Current().ToString()))
                {
                    value = value + input.Current();
                }
                else
                {
                    continuous = false;
                }

                input.MoveNext();
            }

            return tkn = new Token(Kinds.IDENT, value);
        }
    }
}
