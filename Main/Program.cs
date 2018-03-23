using System;
using Lexer.Automaton;
using Lexer.RegularExpression.Impl;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser();
            do
            {
                Console.WriteLine("Please enter a regular expression!");
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == "exit")
                    return;
                try
                {
                    var automaton = parser.Parse(input).Minimize();
                    Console.WriteLine(automaton.StateCount+" states");
                    automaton.Print();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine(e.StackTrace);
                }
            } while (true);
        }
    }
}