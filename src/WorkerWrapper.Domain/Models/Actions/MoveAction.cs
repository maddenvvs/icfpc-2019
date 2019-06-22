using System.Collections.Generic;
using WorkerWrapper.Domain.Geometry;

namespace WorkerWrapper.Domain.Models.Actions
{
    public class MoveAction : IWorkerWrapperAction
    {
        public static readonly Point[] Vectors = new Point[] {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1)
        };

        public static readonly Dictionary<MoveDirection, Point> Dir2Vec = new Dictionary<MoveDirection, Point>()
        {
            {MoveDirection.Right, Vectors[0]},
            {MoveDirection.Down, Vectors[1]},
            {MoveDirection.Left, Vectors[2]},
            {MoveDirection.Up, Vectors[3]},
        };

        public static readonly MoveAction Up = new MoveAction(MoveDirection.Up);

        public static readonly MoveAction Down = new MoveAction(MoveDirection.Down);

        public static readonly MoveAction Left = new MoveAction(MoveDirection.Left);

        public static readonly MoveAction Right = new MoveAction(MoveDirection.Right);

        private MoveAction(MoveDirection direction)
        {
            Direction = direction;
        }

        public MoveDirection Direction { get; }

        public void Execute(ActionContext context)
        {
            throw new System.NotImplementedException();
        }

        public enum MoveDirection
        {
            Up = 0,
            Down,
            Left,
            Right
        }
    }
}