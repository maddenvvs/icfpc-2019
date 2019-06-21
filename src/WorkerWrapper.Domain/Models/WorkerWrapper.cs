using System.Collections.Generic;
using WorkerWrapper.Domain.Geometry;

namespace WorkerWrapper.Domain.Models
{
    public class WorkerWrapper
    {
        private Point _position;
        private Direction _direction;

        private List<IBooster> _boosters;

        public WorkerWrapper(Point initialPosition)
        {
            _position = initialPosition;
            _direction = Direction.Right;
            _boosters = new List<IBooster>();
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