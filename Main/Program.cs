using System;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Please enter a regular expression!");
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == "exit")
                    return;
                
            } while (true);
        }
    }
}