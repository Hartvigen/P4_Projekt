using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandWrittenScannerParser
{

    public class Scanner
    {
        public Inputter input;
        public  Queue<Token> Tokens = new Queue<Token>();
        public static Regex digitRegex = new Regex("[0-9]");
        public static Regex numberRegex = new Regex(digitRegex.ToString() + "|\\.");
        public static Regex IdentFuncRegex = new Regex("[A-Za-z_]");
        
        public Scanner(string path)
        {
            input = new Inputter(path);
        }


        //Dequeues the next token
        public Token DeQueueNext()
        {
            return Tokens.Dequeue();
        }

        //Creates the next token and enqueues it
        public void NextToken()
        {
            Token tkn = null;
            string curstring = input.Current().ToString();

            //Moves through whitespace and \r
            while((string.IsNullOrWhiteSpace(curstring) || curstring[0] == '\r') && input.hasNext)
            {
                input.MoveNext();
                if(input.hasNext)
                    curstring = input.Current().ToString();
            }

            //Checks if the next token will be a number
            if (digitRegex.IsMatch(curstring))
            {
                tkn = NumberToken();
            }
            //Checks if the next token will be a Ident or a func
            else if (IdentFuncRegex.IsMatch(curstring))
            {
                tkn = IdentOrFuncToken();
            }
            else
            {
                if(input.hasNext)
                    //Checks if the next token will be one of the following symbols
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

        //Creates a number token
        public Token NumberToken()
        {
            StringBuilder sb = new StringBuilder();
            string numstring = input.Current().ToString();
            
            //Adds to current char to the string and gets the next one
            while (numberRegex.IsMatch(numstring))
            {
                sb.Append(numstring);
                input.MoveNext();
                numstring = input.Current().ToString();
                //Checks if letters appear in the number
                if (IdentFuncRegex.IsMatch(numstring))
                    throw new Exception($"Error: Number ending at line {input.line}, column {input.col} ends with letters. Might be an Ident or missing a space.");
            }
            //Checks if there are too many dots in the decimal
            if(sb.ToString().Count(f => f == '.') > 1)
                throw new Exception($"Error: Number ending at line {input.line}, column {input.col} contains multiple dots\n");
            return new Token(Kinds.Number, sb.ToString());
        }

        //Creates an Ident of func token
        public Token IdentOrFuncToken()
        {
            StringBuilder sb = new StringBuilder();
            string stringstring = input.Current().ToString();

            //Adds the current char to the string
            while(IdentFuncRegex.IsMatch(stringstring) || digitRegex.IsMatch(stringstring))
            {
                sb.Append(input.Current());
                input.MoveNext();
                stringstring = input.Current().ToString();
            }
            //Checks if the resulting string is "func" and that it is a new words afterwards
            if (string.IsNullOrWhiteSpace(stringstring) && sb.ToString().Equals("func"))
                return new Token(Kinds.Func, sb.ToString());
            else
                return new Token(Kinds.IDENT, sb.ToString());
        }
    }    
}
