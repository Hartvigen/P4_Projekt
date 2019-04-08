namespace HandWrittenScannerParser
{
    public class Token
    {
        private int kind;
        private string value;

        public Token(Kinds kind, string value)
        {
            this.kind = (int)kind;
            this.value = value;
        }

        public string GetValue()
        {
            return value;
        }

        public int Getkind()
        {
            return kind;
        }
    }
}
