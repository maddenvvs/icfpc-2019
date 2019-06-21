using System;

namespace WorkerWrapper.Domain.Geometry
{
    public struct Point
    {
        public Point(int x, int y)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException($"x coordinate is out of range: {x}");
            }

            if (y < 0)
            {
                throw new ArgumentOutOfRangeException($"y coordinate is out of range: {y}");
            }

            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }
    }
}