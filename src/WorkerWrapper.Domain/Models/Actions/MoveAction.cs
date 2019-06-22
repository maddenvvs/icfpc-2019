using System;
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
            var moveVector = Dir2Vec[Direction];
            var worker = context.WorkerWrapper;
            var nextWorkerPos = worker.Position + moveVector;

            if (!context.Mine.IsTileReachable(nextWorkerPos))
            {
                throw new InvalidOperationException($"Cannot move worker at {worker.Position} to position {nextWorkerPos}");
            }

            worker.Position = nextWorkerPos;

            for (var ii = 0; ii < worker.OrangePoints.Count; ii++)
            {
                worker.OrangePoints[ii] = worker.OrangePoints[ii] + moveVector;
            }

            context.Mine.TryColorInYellow(worker.Position, worker.OrangePoints);
        }

        public string Print()
        {
            switch (Direction)
            {
                case MoveDirection.Up: return "W";
                case MoveDirection.Down: return "S";
                case MoveDirection.Left: return "A";
                case MoveDirection.Right: return "D";
                default: throw new Exception($"unknown directions type");
            }
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