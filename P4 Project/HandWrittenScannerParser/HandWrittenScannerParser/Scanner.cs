using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandWrittenScannerParser
{

    class Scanner
    {
        public Inputter input;
        public  Queue<Token> Tokens = new Queue<Token>();
        static Regex digitRegex = new Regex("[0-9]");
        static Regex numberRegex = new Regex(digitRegex.ToString() + "|\\.");
        static Regex IdentFuncRegex = new Regex("[A-Za-z_]");
        static Regex WSREgex = new Regex("\\s | \\r");

        public Scanner(string path)
        {
            input = new Inputter(path);
        }


        public Token Peek()
        {
            return Tokens.Peek();
        }

        public Token DeQueueNext()
        {
            return Tokens.Dequeue();
        }

        public void NextToken()
        {
            Token tkn = null;
            string curstring = input.Current().ToString();

            while((string.IsNullOrWhiteSpace(curstring) || curstring[0] == '\r') && input.hasNext)
            {
                input.MoveNext();
                if(input.hasNext)
                    curstring = input.Current().ToString();
            }

            if (digitRegex.IsMatch(curstring))
            {
                tkn = NumberToken();
            }
            else if (IdentFuncRegex.IsMatch(curstring))
            {
                tkn = IdentOrFuncToken();
            }
            else
            {
                if(input.hasNext)
                    switch (curstring[0])
                    {
                        case '[':
                            tkn = new Token(Kinds.LBrack, "[");
                            break;
                        case ']':
                            tkn = new Token(Kinds.RBrack, "]");
                            break;
                        case '=':
                            tkn = new Token(Kinds.Assign, "=");
                            break;
                        case '(':
                            tkn = new Token(Kinds.LParen, "(");
                            break;
                        case ')':
                            tkn = new Token(Kinds.RParen, ")");
                            break;                       
                        default:
                            throw new Exception("Error: Cannot recognize symbol.");
                    }
                input.MoveNext();
            }
            Tokens.Enqueue(tkn);
        }

        public Token NumberToken()
        {
            StringBuilder sb = new StringBuilder();
            string numstring = input.Current().ToString();
            

            while (numberRegex.IsMatch(numstring))
            {
                sb.Append(numstring);
                input.MoveNext();
                numstring = input.Current().ToString();
            }
            if(sb.ToString().Count(f => f == '.') > 1)
                throw new Exception($"Error: Number ending at line {input.line}, column {input.col} contains multiple dots\n");
            return new Token(Kinds.Number, sb.ToString());
        }

        public Token IdentOrFuncToken()
        {
            StringBuilder sb = new StringBuilder();
            string stringstring = input.Current().ToString();

            while(IdentFuncRegex.IsMatch(stringstring) || digitRegex.IsMatch(stringstring))
            {
                sb.Append(input.Current());
                input.MoveNext();
                stringstring = input.Current().ToString();
            }
            if (string.IsNullOrWhiteSpace(stringstring) && sb.ToString().Equals("func"))
                return new Token(Kinds.Func, sb.ToString());
            else
                return new Token(Kinds.IDENT, sb.ToString());
        }
    }    
}
