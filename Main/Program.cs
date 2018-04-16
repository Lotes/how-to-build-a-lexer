using System;
using Lexer;
using Lexer.Automaton;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new RegexParser();
            do
            {
                Console.WriteLine("Please enter a regular expression!");
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == "exit")
                    return;
                try
                {
                    var automaton = parser.Parse(input);
                    automaton.Print();
                }
                catch (RegexParserException e)
                {
                    Console.WriteLine("  "+(" ".Times(e.Index))+"^");
                    Console.Error.WriteLine(e.Message);
                }
                Console.WriteLine();
            } while (true);
        }
    }
}