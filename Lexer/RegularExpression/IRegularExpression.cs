namespace Lexer.RegularExpression
{
    public interface IRegularExpression
    {
        bool Validate(string haystack);
    }
}