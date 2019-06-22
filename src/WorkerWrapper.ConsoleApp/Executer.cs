using System;
using System.Text;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Optimizer;

namespace WorkerWrapper.ConsoleApp
{
    public class Executer
    {
        private readonly IOptimizer _optimizer;
        private readonly Mine _mine;


        public Executer(Mine mine, IOptimizer optimizer)
        {
            if (optimizer == null || mine == null)
                throw new ArgumentNullException();

            _optimizer = optimizer;
            _mine = mine;
        }

        public string PrintActionSequence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var workerWrapperAction in _optimizer.FindSequence(_mine))
            {
                stringBuilder.Append(workerWrapperAction.Print());
            }

            return stringBuilder.ToString();
        }
    }
}