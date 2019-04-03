using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HandWrittenScannerParser
{
    class Inputter
    {
        public int line = 1;
        public int col = 0;
        private IEnumerator<char> input;
        public bool hasNext = true;

        public Inputter(string path)
        {
            input = File.ReadAllText(path, Encoding.UTF8).GetEnumerator();
            input.MoveNext();
        }

        public void MoveNext()
        {
            if (hasNext) { 
            hasNext = input.MoveNext();
            col++;
                if (hasNext && input.Current.ToString()[0].Equals('\n'))
                {
                    line++;
                    hasNext = input.MoveNext();
                    col = 1;
                }
            }
            
        }

        public char Current()
        {
            if (hasNext)
                return input.Current;
            else
                return '\n';
            
        }

    }
}
