using System;
using System.Collections.Generic;
using WorkerWrapper.Domain.Geometry;
using WorkerWrapper.Domain.Models.Actions;

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

            ColorMap = new Dictionary<Point, MineColor>();

            InitializeMapColor();
        }

        public Polygon Contour { get; }

        public WorkerWrapper Worker { get; }

        public List<Polygon> Obstacles { get; }

        public Dictionary<Point, IBooster> Boosters { get; }

        public Dictionary<Point, MineColor> ColorMap { get; }

        public void InitializeMapColor()
        {
            foreach (var p in Contour.OuterBorderPoints())
            {
                ColorMap[p] = MineColor.Black;
            }

            foreach (var obstacle in Obstacles)
            {
                foreach (var p in obstacle.InnerBorderPoints())
                {
                    ColorMap[p] = MineColor.Black;
                }
            }

            FillMoveableTiles();
        }

        public bool IsTileReachable(Point p) =>
            ColorMap.ContainsKey(p) && ColorMap[p] != MineColor.Black;

        public void TryColorInYellow(Point workerPosition, List<Point> orangePoints)
        {
            // Simple one, we don't use any boosters, so orange point is visible
            // if it is not blacked
            foreach (var point in orangePoints)
            {
                if (!this.IsTileReachable(point))
                {
                    continue;
                }
                if (ColorMap[point] != MineColor.Yellow)
                {
                    ColorMap[point] = MineColor.Yellow;
                    // TODO: increment count of colored tiles
                }
            }
        }

        private void FillMoveableTiles()
        {
            var stack = new Stack<Point>();
            stack.Push(Worker.Position);
            ColorMap[Worker.Position] = MineColor.Grey;

            while (stack.Count > 0)
            {
                var currPoint = stack.Pop();

                foreach (var vector in MoveAction.Vectors)
                {
                    var nextPoint = currPoint + vector;

                    if (!ColorMap.ContainsKey(nextPoint))
                    {
                        stack.Push(nextPoint);
                        ColorMap[nextPoint] = MineColor.Grey;

                        // TODO: increment counter of total tiles to color
                    }
                }
            }
        }
    }
}