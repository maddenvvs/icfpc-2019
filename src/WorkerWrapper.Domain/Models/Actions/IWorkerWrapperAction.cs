namespace WorkerWrapper.Domain.Models.Actions
{
    public interface IWorkerWrapperAction
    {
        void Execute(ActionContext context);
    }

    public class ActionContext
    {
        public WorkerWrapper WorkerWrapper { get; set; }

        public Mine Mine { get; set; }
    }
}