namespace Lexer
{
    public interface IToken
    {
        int Index { get; }
        string Value { get; }
        ITokenType TokenType { get; }
    }
}
