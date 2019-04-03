using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandWrittenScannerParser
{
    class Parser
    {
        Token CurrentTerminal;
        Scanner scanner;

        public Parser(string path)
        {
            scanner = new Scanner(path);
            scanner.NextToken();
            CurrentTerminal = scanner.DeQueueNext();
        }

        public void parse()
        {
            try {
                parseMAGIA();
                Console.WriteLine("We did it, kupo!");
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void accept(Kinds expected)
        {
            Console.WriteLine(CurrentTerminal.GetValue());
            if (CurrentTerminal.Getkind() == (int)expected)
            {
                scanner.NextToken();
                CurrentTerminal = scanner.DeQueueNext();
            }
            else
                throw new Exception($"Error Cannot parse token: kind {CurrentTerminal.Getkind()}, value {CurrentTerminal.GetValue()}\n");
        }

        private void parseMAGIA()
        {
            while(CurrentTerminal.Getkind() == (int) Kinds.LBrack)
                parseHead();
            ParseStms();
            while(CurrentTerminal != null && CurrentTerminal.Getkind() == (int) Kinds.Func)
                ParseFuncDecl();
        }
        private void parseHead()
        {
            accept(Kinds.LBrack);
            accept(Kinds.Number);
            accept(Kinds.RBrack);
        }
        private void ParseStms()
        {
            accept(Kinds.IDENT);
            accept(Kinds.Assign);
            accept(Kinds.IDENT);
        }
        private void ParseFuncDecl()
        {
            accept(Kinds.Func);
            accept(Kinds.IDENT);
            accept(Kinds.LParen);
            accept(Kinds.RParen);
            ParseStms();
        }

    }
}
