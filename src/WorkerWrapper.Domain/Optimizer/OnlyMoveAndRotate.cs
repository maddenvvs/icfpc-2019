using System.Collections.Generic;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Actions;

namespace WorkerWrapper.Domain.Optimizer
{
    public class OnlyMoveAndRotate : IOptimizer
    {
        public IEnumerable<IWorkerWrapperAction> FindSequence(Mine mine)
        {
            throw new System.NotImplementedException();
        }
    }
}