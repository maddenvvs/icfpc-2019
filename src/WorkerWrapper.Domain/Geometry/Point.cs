using System;

namespace WorkerWrapper.Domain.Geometry
{
    public struct Point : IEquatable<Point>
    {
        public static readonly Point Origin = new Point(0, 0);

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public override int GetHashCode()
        {
            unchecked
            {
                return (32768 * Y) + X;
            }
        }

        public override bool Equals(object other)
        {
            return Equals((Point)other);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
            => $"({X},{Y})";

        public static Point operator +(Point first, Point second)
        {
            return new Point(first.X + second.X, first.Y + second.Y);
        }

        public Point Move(Point vector)
        {
            return new Point(X + vector.X, Y + vector.Y);
        }

        /// <summary>
        /// Rotates one point around another
        /// Taken from https://stackoverflow.com/questions/13695317/rotate-a-point-around-another-point
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The center point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        public Point Rotate(Point centerPoint, Rotation rotation)
        {
            int sinTheta = rotation == Rotation.Clockwise ? -1 : 1;
            return new Point(
                    (-sinTheta * (Y - centerPoint.Y) + centerPoint.X),
                    (sinTheta * (X - centerPoint.X) + centerPoint.Y)
            );
        }

        public Point RotateAroundOrigin(Rotation rotation)
        {
            return Rotate(Origin, rotation);
        }
    }
}