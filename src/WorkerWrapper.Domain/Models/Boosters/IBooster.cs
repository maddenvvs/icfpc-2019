namespace WorkerWrapper.Domain.Models
{
    public interface IBooster
    {
        void Apply(BoosterContext context);
    }

    public class BoosterContext
    {
        public WorkerWrapper WorkerWrapper { get; set; }
    }
}