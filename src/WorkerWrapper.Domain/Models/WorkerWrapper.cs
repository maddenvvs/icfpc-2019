using System.Collections.Generic;
using WorkerWrapper.Domain.Geometry;

namespace WorkerWrapper.Domain.Models
{
    public class WorkerWrapper
    {
        public WorkerWrapper(Point initialPosition)
        {
            Position = initialPosition;
            Direction = Actions.MoveAction.Direction.Right;
            Boosters = new List<IBooster>();
            OrangePoints = new List<Point> {
                initialPosition,
                initialPosition + new Point(1, 0),
                initialPosition + new Point(1, 1),
                initialPosition + new Point(1, -1)
            };
        }

        public Point Position { get; set; }

        public List<Point> OrangePoints { get; set; }

        public Actions.MoveAction.Direction Direction { get; set; }

        public List<IBooster> Boosters { get; set; }
    }
}