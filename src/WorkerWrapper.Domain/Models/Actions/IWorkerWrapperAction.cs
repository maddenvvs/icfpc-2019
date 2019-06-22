namespace WorkerWrapper.Domain.Models.Actions
{
    public interface IWorkerWrapperAction
    {
        void Execute(ActionContext context);

        string Print();
    }

    public class ActionContext
    {
        public WorkerWrapper WorkerWrapper { get; set; }

        public Mine Mine { get; set; }
    }
}