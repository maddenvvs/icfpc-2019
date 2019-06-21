using System.Collections.Generic;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Actions;

namespace WorkerWrapper.Domain.Optimizer
{
    public interface IOptimizer
    {
        IEnumerable<IWorkerWrapperAction> FindSequence(Mine mine);
    }
}