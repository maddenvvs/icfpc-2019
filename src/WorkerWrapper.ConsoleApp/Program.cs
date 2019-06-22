using System;
using System.IO;
using WorkerWrapper.Domain.Optimizer;

namespace WorkerWrapper.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        //args[0] optimizer key
        //OMR -OnlyMoveAndRotate
        {
            //Console.WriteLine("paste description");

            string description = Console.ReadLine();
            // string description = File.ReadAllText("/Users/denis/Downloads/part-1-initial/prob-001.desc").Trim();
            var parser = new Parser(description);
            var mine = parser.ConfigMine();

            string optKey = string.Empty;
            if (args?.Length != 0)
                optKey = args[0];

            var optimizer = OptimizerFactory.CreateOptimizer(optKey);
            var executer = new Executer(mine, optimizer);

            Console.WriteLine(executer.PrintActionSequence());

            Console.ReadLine();
        }
    }
}
