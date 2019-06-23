using System;
using System.Collections.Generic;
using System.Linq;
using WorkerWrapper.Domain.DataStructures;
using WorkerWrapper.Domain.Geometry;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Actions;

namespace WorkerWrapper.Domain.Optimizer
{
    public class OnlyMoveAndRotateDijkstra : IOptimizer
    {
        private Mine Mine { get; set; }

        private Dictionary<Point, int> Point2IslandSize { get; set; } = new Dictionary<Point, int>();

        public IEnumerable<IWorkerWrapperAction> FindSequence(Mine mine)
        {
            Mine = mine;

            while (mine.HasTilesToPaint)
            {
                var currentPoint = mine.Worker.Position;

                var (nextPoint, pathToPoint) = FindNearestTileToPaint(mine, currentPoint);
                var actionsSequence = BuildActionSequence(mine, currentPoint, pathToPoint);

                var context = new ActionContext
                {
                    Mine = mine,
                    WorkerWrapper = mine.Worker,
                };

                foreach (var action in actionsSequence)
                {
                    action.Execute(context);

                    yield return action;

                    // TODO: if it paints next point during movement
                    // we can early exit
                    if (mine.IsPainted(nextPoint)) break;
                }
            }
        }

        private IEnumerable<IWorkerWrapperAction> BuildActionSequence(
            Mine mine,
            Point initialPoint,
            List<Point> pathToPoint)
        {
            var currentPoint = initialPoint;
            var currentLookDirection = mine.Worker.Direction;

            var needsRotation = pathToPoint.Count < 5;

            for (var ii = 1; ii < pathToPoint.Count; ii++)
            {
                var nextPoint = pathToPoint[ii];

                if (needsRotation)
                {
                    RotateAction.LookDirection avaitableDirection;

                    if (currentPoint.X == nextPoint.X)
                    {
                        avaitableDirection = currentPoint.Y < nextPoint.Y ?
                            RotateAction.LookDirection.Up :
                            RotateAction.LookDirection.Down;
                    }
                    else
                    {
                        avaitableDirection = currentPoint.X < nextPoint.X ?
                            RotateAction.LookDirection.Right :
                            RotateAction.LookDirection.Left;
                    }

                    if (currentLookDirection != avaitableDirection)
                    {
                        var currentLookInt = (int)currentLookDirection;
                        var avaitableLookInt = (int)avaitableDirection;

                        var rotations = RotateAction.RotationMatrix[currentLookInt][avaitableLookInt];
                        var numberOfRotations = Math.Abs(rotations);

                        for (var k = 0; k < numberOfRotations; k++)
                        {
                            if (rotations > 0)
                            {
                                yield return RotateAction.Clockwise;
                            }
                            else
                            {
                                yield return RotateAction.CounterClockwise;
                            }
                        }
                        currentLookDirection = avaitableDirection;
                    }
                }


                if (currentPoint.X == nextPoint.X)
                {
                    if (currentPoint.Y < nextPoint.Y)
                    {
                        yield return MoveAction.Up;
                    }
                    else
                    {
                        yield return MoveAction.Down;
                    }
                }
                else
                {
                    if (currentPoint.X < nextPoint.X)
                    {
                        yield return MoveAction.Right;
                    }
                    else
                    {
                        yield return MoveAction.Left;
                    }
                }

                currentPoint = pathToPoint[ii];
            }
        }

        private (Point, List<Point>) FindNearestTileToPaint(Mine mine, Point initialPoint)
        {
            return FindNearestTileToPaintDijkstra(mine, initialPoint);
        }

        private static readonly IComparer<(int, Point)> _defaultComparer =
            Comparer<(int, Point)>.Create((l, r) => l.Item1 - r.Item1);

        private (Point, List<Point>) FindNearestTileToPaintDijkstra(Mine mine, Point initialPoint)
        {
            Point2IslandSize.Clear();

            var distances = new Dictionary<Point, int>();
            distances[initialPoint] = 0;

            var lookDirections = new Dictionary<Point, RotateAction.LookDirection>();
            lookDirections[initialPoint] = mine.Worker.Direction;

            var parent = new Dictionary<Point, Point>();
            parent[initialPoint] = initialPoint;

            var heap = new BinaryHeap<(int, Point)>(_defaultComparer);
            heap.Add((0, initialPoint));

            var bestTileToInvestigate = Point.Origin;
            var bestEstimationSoFar = int.MaxValue;
            int? nearestEstimatedDistance = null;

            while (heap.Count > 0)
            {
                var (startDist, startPoint) = heap.ExtractMin();

                if (nearestEstimatedDistance.HasValue)
                {
                    if (startDist > nearestEstimatedDistance + 30) break;
                }

                if (mine.NeedsToBePainted(startPoint))
                {
                    if (!nearestEstimatedDistance.HasValue)
                    {
                        nearestEstimatedDistance = startDist;
                    }

                    var estimationOfPoint = EstimateNeedToBePaintedIslandAt(startPoint);
                    if (estimationOfPoint < bestEstimationSoFar)
                    {
                        bestEstimationSoFar = estimationOfPoint;
                        bestTileToInvestigate = startPoint;
                    }
                }

                var startPointLook = lookDirections[startPoint];
                var vecSuggestions = RotateAction.Look2VecSuggestion[startPointLook];

                for (var ii = 0; ii < 4; ii++)
                {
                    var nextPoint = startPoint + vecSuggestions[ii];

                    if (!mine.IsTileReachable(nextPoint)) continue;

                    var nextPointMoveCost = 0;

                    if (ii == 0)
                    {
                        nextPointMoveCost = 1;
                    }
                    else if (ii == 3)
                    {
                        nextPointMoveCost = 3;
                    }
                    else
                    {
                        nextPointMoveCost = 2;
                    }

                    var nextPointDist = startDist + nextPointMoveCost;
                    if (!distances.ContainsKey(nextPoint) || nextPointDist < distances[nextPoint])
                    {
                        distances[nextPoint] = nextPointDist;
                        parent[nextPoint] = startPoint;

                        lookDirections[nextPoint] = RotateAction.Vec2Look[vecSuggestions[ii]];

                        heap.Add((nextPointDist, nextPoint));
                    }
                }
            }

            var path = new List<Point>();

            var curr = bestTileToInvestigate;
            while (!curr.Equals(parent[curr]))
            {
                path.Add(curr);
                curr = parent[curr];
            }

            path.Add(initialPoint);

            path.Reverse();

            return (bestTileToInvestigate, path);
        }

        private int EstimateNeedToBePaintedIslandAt(Point point)
        {
            if (Point2IslandSize.ContainsKey(point))
            {
                return Point2IslandSize[point];
            }

            var visited = new HashSet<Point>();
            var stack = new Stack<Point>();
            stack.Push(point);
            visited.Add(point);

            var estimation = 1;

            while (stack.Count > 0)
            {
                var currPoint = stack.Pop();

                foreach (var vector in MoveAction.Vectors)
                {
                    var nextPoint = currPoint + vector;

                    if (visited.Contains(nextPoint)) continue;

                    if (Mine.NeedsToBePainted(nextPoint))
                    {
                        stack.Push(nextPoint);
                        visited.Add(nextPoint);

                        estimation++;
                    }
                }
            }

            foreach (var p in visited)
            {
                Point2IslandSize[p] = estimation;
            }

            return estimation;
        }
    }
}