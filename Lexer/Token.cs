namespace Lexer
{
    public class Token : IToken
    {
        public Token(ITokenType type, string value, int index)
        {
            Index = index;
            Value = value;
            TokenType = type;
        }
        public int Index { get; }
        public string Value { get; }
        public ITokenType TokenType { get; }
        public override string ToString()
        {
            return "\""+Value+"\": "+TokenType.Name;
        }
    }
}
