using System;

namespace WorkerWrapper.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("paste description");
            string description = Console.ReadLine();
            var parser = new Parser(description);
            var mine = parser.ConfigMine();
            Console.ReadLine();
        }
    }
}
