namespace WorkerWrapper.Domain.Optimizer
{
    public static class OptimizerFactory
    {
        public static IOptimizer CreateOptimizer(string key)
        {
            // if (string.IsNullOrEmpty(key))
            // return new OnlyMoveAndRotate();

            switch (key)
            {
                //TODO add new optimizers here
                case "OMR":
                default:
                    return new ChooseBestOfAll();
            }
        }
    }
}