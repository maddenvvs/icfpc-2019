using System;
using System.Collections.Generic;
using System.Linq;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Actions;

namespace WorkerWrapper.Domain.Optimizer
{
    public class ChooseBestOfAll : IOptimizer
    {
        private readonly IEnumerable<IOptimizer> _optimizers;

        public ChooseBestOfAll()
            : this(new IOptimizer[] {
                new OnlyMoveAndRotate(),
                new OnlyMoveAndRotateV2(),
                new OnlyMoveAndRotateV3(),
                new OnlyMoveAndRotateDijkstra(),
            })
        {
        }

        public ChooseBestOfAll(IEnumerable<IOptimizer> optimizers)
        {
            if (optimizers == null)
            {
                throw new ArgumentNullException(nameof(optimizers));
            }

            _optimizers = optimizers;
        }

        public IEnumerable<IWorkerWrapperAction> FindSequence(Mine mine)
        {
            var bestScore = int.MaxValue;
            var bestMoves = (List<IWorkerWrapperAction>)null;

            foreach (var optimizer in _optimizers)
            {
                var moves = optimizer.FindSequence(mine.Clone()).ToList();
                if (moves.Count < bestScore)
                {
                    bestScore = moves.Count;
                    bestMoves = moves;
                }
            }

            return bestMoves;
        }
    }
}