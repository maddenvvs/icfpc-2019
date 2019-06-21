using System;
using System.Collections.Generic;
using System.Linq;
using WorkerWrapper.Domain.Geometry;

namespace WorkerWrapper.Domain.Models
{
    public class Mine
    {
        public Mine(
            Polygon contour,
            Point workerPosition,
            List<Polygon> obstacles,
            Dictionary<Point, IBooster> boosters
        )
        {
            if (contour == null)
            {
                throw new ArgumentNullException(nameof(contour));
            }
            if (obstacles == null)
            {
                throw new ArgumentNullException(nameof(obstacles));
            }
            if (boosters == null)
            {
                throw new ArgumentNullException(nameof(boosters));
            }

            Contour = contour;
            Worker = new WorkerWrapper(workerPosition);
            Obstacles = obstacles;
            Boosters = boosters;
        }

        public Polygon Contour { get; }

        public WorkerWrapper Worker { get; }

        public List<Polygon> Obstacles { get; }

        public Dictionary<Point, IBooster> Boosters { get; }
    }
}