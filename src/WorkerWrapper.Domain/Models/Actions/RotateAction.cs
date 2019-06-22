using WorkerWrapper.Domain.Geometry;

namespace WorkerWrapper.Domain.Models.Actions
{
    public class RotateAction : IWorkerWrapperAction
    {
        public static readonly RotateAction Clockwise = new RotateAction(Rotation.Clockwise);

        public static readonly RotateAction CounterClockwise = new RotateAction(Rotation.CounterClockwise);

        public static readonly int[][] RotationMatrix = new int[][] {
            new int[] {0, 1, 2, -1},
            new int[] {-1, 0, 1, 2},
            new int[] {-2, -1, 0, 1},
            new int[] {1, -2, -1, 0},
        };

        private RotateAction(Rotation rotation)
        {
            Rotation = rotation;
        }

        public Rotation Rotation { get; }

        public void Execute(ActionContext context)
        {
            var worker = context.WorkerWrapper;

            for (var ii = 0; ii < worker.OrangePoints.Count; ii++)
            {
                worker.OrangePoints[ii] =
                    worker.OrangePoints[ii].Rotate(worker.Position, Rotation);
            }

            var diff = Rotation == Rotation.Clockwise ? 1 : -1;
            worker.Direction = (RotateAction.LookDirection)((((int)worker.Direction + diff) % 4 + 4) % 4);

            context.Mine.TryColorInYellow(worker.Position, worker.OrangePoints);
        }

        public string Print()
        {
            return Rotation == Rotation.Clockwise ? "E" : "Q";
        }

        // If we change Up to -1, then rotation matrix can be replaced to
        // nextDirection - currentDirection
        public enum LookDirection
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3
        }
    }
}