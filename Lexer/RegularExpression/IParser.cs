namespace Lexer.RegularExpression
{
    public interface IParser
    {
        IRegularExpression Parse(string input);
    }
}