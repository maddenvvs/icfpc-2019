using System;
using System.Collections.Generic;
using System.Linq;
using WorkerWrapper.Domain.Geometry;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Actions;

namespace WorkerWrapper.Domain.Optimizer
{
    public class OnlyMoveAndRotateV2 : IOptimizer
    {
        public Mine Mine { get; private set; }

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

            for (var ii = 1; ii < pathToPoint.Count; ii++)
            {
                var nextPoint = pathToPoint[ii];
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
            var queue = new Queue<(int, Point)>();
            queue.Enqueue((0, initialPoint));

            var parent = new Dictionary<Point, Point>();
            parent[initialPoint] = initialPoint;

            var initialLookDirection = mine.Worker.Direction;
            var preferredDirections = RotateAction.Look2VecSuggestion[Mine.Worker.Direction];

            var minLengthToPaint = int.MaxValue;
            var potentialNextPoints = new List<Point>();

            while (queue.Count > 0)
            {
                var (pathLength, startPoint) = queue.Dequeue();

                if (pathLength > minLengthToPaint) continue;

                foreach (var vector in preferredDirections)
                {
                    var nextPoint = startPoint + vector;

                    if (parent.ContainsKey(nextPoint)) continue;

                    if (mine.NeedsToBePainted(nextPoint))
                    {
                        minLengthToPaint = pathLength + 1;
                        potentialNextPoints.Add(nextPoint);

                        parent[nextPoint] = startPoint;
                    }

                    if (mine.IsPainted(nextPoint))
                    {
                        queue.Enqueue((pathLength + 1, nextPoint));
                        parent[nextPoint] = startPoint;
                    }
                }
            }

            var minPoint = Point.Origin;
            var minEstimation = int.MaxValue;

            foreach (var point in potentialNextPoints)
            {
                var estimation = EstimateNeedToBePaintedIslandAt(point);
                if (estimation < minEstimation)
                {
                    minEstimation = estimation;
                    minPoint = point;
                }
            }

            var path = new List<Point>();

            var curr = minPoint;
            while (!curr.Equals(parent[curr]))
            {
                path.Add(curr);
                curr = parent[curr];
            }

            path.Add(initialPoint);
            path.Reverse();

            return (minPoint, path);
        }

        private int EstimateNeedToBePaintedIslandAt(Point point)
        {
            var visited = new HashSet<Point>();
            var stack = new Stack<Point>();
            stack.Push(point);
            visited.Add(point);

            var estimation = 1;

            while (stack.Count > 0 && estimation < 100)
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

            return estimation;
        }
    }
}