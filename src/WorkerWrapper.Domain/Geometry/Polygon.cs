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
    }
}