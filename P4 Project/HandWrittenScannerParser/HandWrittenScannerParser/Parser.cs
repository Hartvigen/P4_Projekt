using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandWrittenScannerParser
{
    public class Parser
    {
        public Token CurrentTerminal;
        Scanner scanner;

        public Parser(string path)
        {
            scanner = new Scanner(path);
            scanner.NextToken();
            CurrentTerminal = scanner.DeQueueNext();
        }        
        
        //Checks if the next token is the expected one
        public void accept(Kinds expected)
        {
            Console.WriteLine(CurrentTerminal.GetValue());
            //if it is the expected token, the next one is loaded
            if (CurrentTerminal.Getkind() == (int)expected)
            {
                scanner.NextToken();
                CurrentTerminal = scanner.DeQueueNext();
            }
            //otherwise, an exception is thrown.
            else
                throw new Exception($"Error Cannot parse token: kind {CurrentTerminal.Getkind()}, value {CurrentTerminal.GetValue()}\n");
        }

        //Parses an entire MAGIA-lite program
        public void ParseMAGIA()
        {
            //Checks for the start of a header
            while(CurrentTerminal.Getkind() == (int) Kinds.LBrack)
                ParseHead();
            ParseStmts();
            //Could have a check for a func, but for testing, that was removed.
            while(CurrentTerminal != null)
                ParseFuncDecl();
        }
        //Parses a head
        public void ParseHead()
        {
            accept(Kinds.LBrack);
            accept(Kinds.Number);
            accept(Kinds.RBrack);
        }
        //Parses Stmts
        public void ParseStmts()
        {
            accept(Kinds.IDENT);
            accept(Kinds.Assign);
            accept(Kinds.IDENT);
        }
        //Parses a func decl
        public void ParseFuncDecl()
        {
            accept(Kinds.Func);
            accept(Kinds.IDENT);
            accept(Kinds.LParen);
            accept(Kinds.RParen);
            ParseStmts();
        }

    }
}
