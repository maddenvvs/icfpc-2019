using System;
using WorkerWrapper.Domain.Geometry;
using Xunit;

namespace WorkerWrapper.Domain.Tests
{
    public class SomePoint
    {
        private readonly Point _point;

        public SomePoint()
        {
            _point = new Point(3, 4);
        }

        [Fact]
        public void DoesntChangeIfRotatesClockwiseAroundItself()
        {
            var rotated = _point.Rotate(_point, Rotation.Clockwise);

            Assert.Equal(3, rotated.X);
            Assert.Equal(4, rotated.Y);
        }

        [Fact]
        public void DoesntChangeIfRotatesCounterClockwiseAroundItself()
        {
            var rotated = _point.Rotate(_point, Rotation.CounterClockwise);

            Assert.Equal(3, rotated.X);
            Assert.Equal(4, rotated.Y);
        }

        [Fact]
        public void ChangesIfRotatesClockwiseAroundFirstPoint()
        {
            var firstPoint = new Point(3, 3);
            var rotated = _point.Rotate(firstPoint, Rotation.Clockwise);

            Assert.Equal(4, rotated.X);
            Assert.Equal(3, rotated.Y);
        }

        [Fact]
        public void ChangesIfRotatesCounterClockwiseAroundFirstPoint()
        {
            var firstPoint = new Point(3, 3);
            var rotated = _point.Rotate(firstPoint, Rotation.CounterClockwise);

            Assert.Equal(2, rotated.X);
            Assert.Equal(3, rotated.Y);
        }

        [Fact]
        public void ChangesIfRotatesClockwiseAroundSecondPoint()
        {
            var firstPoint = new Point(5, 3);
            var rotated = _point.Rotate(firstPoint, Rotation.Clockwise);

            Assert.Equal(6, rotated.X);
            Assert.Equal(5, rotated.Y);
        }

        [Fact]
        public void ChangesIfRotatesCounterClockwiseAroundSecondPoint()
        {
            var firstPoint = new Point(5, 3);
            var rotated = _point.Rotate(firstPoint, Rotation.CounterClockwise);

            Assert.Equal(4, rotated.X);
            Assert.Equal(1, rotated.Y);
        }
    }
}
