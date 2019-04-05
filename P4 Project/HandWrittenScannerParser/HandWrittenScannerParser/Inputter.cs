using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HandWrittenScannerParser
{
    public class Inputter
    {
        public int line;
        public int col;
        public IEnumerator<char> ProgramString;
        public bool hasNext = true;

        public Inputter(string path)
        {
            ProgramString = File.ReadAllText(path, Encoding.UTF8).GetEnumerator();
            line = 1;
            col = 0;
            ProgramString.MoveNext();
            col++;
        }

        //Moves to the next char and updates the col and line.
        public void MoveNext()
        {
            if (hasNext) { 
            hasNext = ProgramString.MoveNext();
            col++;
                //If a new line, reset col
                if (hasNext && ProgramString.Current.ToString()[0].Equals('\n'))
                {
                    line++;
                    hasNext = ProgramString.MoveNext();
                    col = 1;
                }
            }
            
        }
        
        //Gets the current char. If EOF, return \n
        public char Current()
        {
            if (hasNext)
                return ProgramString.Current;
            else
                return '\n';
            
        }

    }
}
