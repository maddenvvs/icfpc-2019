using System;
using System.Collections.Generic;
using System.Linq;
using WorkerWrapper.Domain.Geometry;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Actions;

namespace WorkerWrapper.Domain.Optimizer
{
    public class OnlyMoveAndRotateV3 : IOptimizer
    {
        public IEnumerable<IWorkerWrapperAction> FindSequence(Mine mine)
        {
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
            var needsRotation = pathToPoint.Count > 3;

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
            var queue = new Queue<Point>();
            queue.Enqueue(initialPoint);
            var parent = new Dictionary<Point, Point>();
            parent[initialPoint] = initialPoint;

            while (queue.Count > 0)
            {
                var startPoint = queue.Dequeue();

                foreach (var vector in MoveAction.Vectors)
                {
                    var nextPoint = startPoint + vector;

                    if (parent.ContainsKey(nextPoint)) continue;

                    if (mine.NeedsToBePainted(nextPoint))
                    {
                        var path = new List<Point>();
                        path.Add(nextPoint);

                        var curr = startPoint;
                        while (!curr.Equals(parent[curr]))
                        {
                            path.Add(curr);
                            curr = parent[curr];
                        }

                        path.Add(initialPoint);

                        path.Reverse();

                        return (nextPoint, path);
                    }

                    if (mine.IsPainted(nextPoint))
                    {
                        queue.Enqueue(nextPoint);
                        parent[nextPoint] = startPoint;
                    }
                }
            }

            return (Point.Origin, null);
        }
    }
}