namespace WorkerWrapper.Domain.Models.Actions
{
    public class MoveAction : IWorkerWrapperAction
    {
        public void Execute(ActionContext context)
        {
            throw new System.NotImplementedException();
        }

        public enum Direction
        {
            Right = 0,
            Down,
            Left,
            Top
        }
    }
}