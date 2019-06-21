using WorkerWrapper.Domain.Geometry;

namespace WorkerWrapper.Domain.Models.Actions
{
    public class RotateAction : IWorkerWrapperAction
    {
        public static readonly RotateAction Clockwise = new RotateAction(Rotation.Clockwise);

        public static readonly RotateAction CounterClockwise = new RotateAction(Rotation.CounterClockwise);

        private readonly Rotation _rotation;

        private RotateAction(Rotation rotation)
        {
            _rotation = rotation;
        }
        public void Execute(ActionContext context)
        {
            var worker = context.WorkerWrapper;

            for (var ii = 0; ii < worker.OrangePoints.Count; ii++)
            {
                worker.OrangePoints[ii] =
                    worker.OrangePoints[ii].Rotate(worker.Position, _rotation);
            }

            var diff = _rotation == Rotation.Clockwise ? 1 : -1;
            worker.Direction = (MoveAction.Direction)((((int)worker.Direction + diff) % 4 + 4) % 4);

            // TODO: update mine yellow cells
        }
    }
}