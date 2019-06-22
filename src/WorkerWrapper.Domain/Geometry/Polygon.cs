using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkerWrapper.Domain.Geometry
{
    public class Polygon
    {
        private readonly Point[] _points;

        public Polygon(ICollection<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            _points = points.ToArray();
        }

        private Polygon(Point[] points)
        {
            _points = points;
        }

        public Polygon Move(Point vector)
        {
            var movedPoints = new Point[_points.Length];
            for (var ii = 0; ii < _points.Length; ii++)
            {
                movedPoints[ii] = _points[ii].Move(vector);
            }

            return new Polygon(movedPoints);
        }

        public Polygon MoveInPlace(Point vector)
        {
            for (var ii = 0; ii < _points.Length; ii++)
            {
                _points[ii] = _points[ii].Move(vector);
            }

            return this;
        }

        public Polygon RotateAtPivot(Point pivot, Rotation rotation)
        {
            var rotatedPoints = new Point[_points.Length];
            for (var ii = 0; ii < _points.Length; ii++)
            {
                rotatedPoints[ii] = _points[ii].Rotate(pivot, rotation);
            }

            return new Polygon(rotatedPoints);
        }

        public Polygon RotateAtPivotInPlace(Point pivot, Rotation rotation)
        {
            for (var ii = 0; ii < _points.Length; ii++)
            {
                _points[ii] = _points[ii].Rotate(pivot, rotation);
            }

            return this;
        }

        public IEnumerable<Point> InnerBorderPoints()
        {
            var pointsLength = _points.Length;

            for (var ii = 0; ii < pointsLength; ii++)
            {
                var firstPoint = _points[ii];
                var secondPoint = _points[(ii + 1) % pointsLength];

                if (firstPoint.X == secondPoint.X)
                {
                    // go down
                    if (firstPoint.Y > secondPoint.Y)
                    {
                        for (var y = firstPoint.Y - 1; y >= secondPoint.Y; y--)
                        {
                            yield return new Point(firstPoint.X, y);
                        }
                    }
                    // go up
                    else
                    {
                        for (var y = firstPoint.Y; y < secondPoint.Y; y++)
                        {
                            yield return new Point(firstPoint.X - 1, y);
                        }
                    }
                }
                // go left
                else if (firstPoint.X > secondPoint.X)
                {
                    for (var x = firstPoint.X - 1; x >= secondPoint.X; x--)
                    {
                        yield return new Point(x, firstPoint.Y - 1);
                    }
                }
                // go right
                else
                {
                    for (var x = firstPoint.X; x < secondPoint.X; x++)
                    {
                        yield return new Point(x, firstPoint.Y);
                    }
                }
            }
        }

        public IEnumerable<Point> OuterBorderPoints()
        {
            var pointsLength = _points.Length;

            for (var ii = 0; ii < pointsLength; ii++)
            {
                var firstPoint = _points[ii];
                var secondPoint = _points[(ii + 1) % pointsLength];

                if (firstPoint.X == secondPoint.X)
                {
                    // go down
                    if (firstPoint.Y > secondPoint.Y)
                    {
                        for (var y = firstPoint.Y - 1; y >= secondPoint.Y; y--)
                        {
                            yield return new Point(firstPoint.X - 1, y);
                        }
                    }
                    // go up
                    else
                    {
                        for (var y = firstPoint.Y; y < secondPoint.Y; y++)
                        {
                            yield return new Point(firstPoint.X, y);
                        }
                    }
                }
                // go left
                else if (firstPoint.X > secondPoint.X)
                {
                    for (var x = firstPoint.X - 1; x >= secondPoint.X; x--)
                    {
                        yield return new Point(x, firstPoint.Y);
                    }
                }
                // go right
                else
                {
                    for (var x = firstPoint.X; x < secondPoint.X; x++)
                    {
                        yield return new Point(x, firstPoint.Y - 1);
                    }
                }
            }
        }

    }
}