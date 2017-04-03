﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SixLabors.Shapes.Tests
{
    using System.Numerics;

    public class StarTests
    {

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        public void RequiresAtleast3Verticies(int points, bool throws)
        {
            if (throws)
            {
                var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Star(Vector2.Zero, points, 10f, 20f, 0));

                Assert.Equal("prongs", ex.ParamName);
            }
            else
            {
                var p = new Star(Vector2.Zero, points, 10f, 20f, 0);
                Assert.NotNull(p);
            }
        }

        [Theory]
        [InlineData(-1, true)]
        [InlineData(0, true)]
        [InlineData(0.00001, false)]
        [InlineData(1, false)]
        public void RadiusMustBeGreateThan0(float radius, bool throws)
        {
            if (throws)
            {
                var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Star(Vector2.Zero, 3, radius, 20f, 0));
                var ex2 = Assert.Throws<ArgumentOutOfRangeException>(() => new Star(Vector2.Zero, 3, 20f, radius, 0));

                Assert.Equal("innerRadii", ex.ParamName);
                Assert.Equal("outerRadii", ex2.ParamName);
            }
            else
            {
                var p = new Star(Vector2.Zero, 3, radius, radius, 0);
                Assert.NotNull(p);
            }
        }

        [Fact]
        public void GeneratesCorrectPath()
        {
            float radius = 5;
            float radius2 = 30;
            int pointsCount = new Random().Next(3, 20);

            var poly = new Star(Vector2.Zero, pointsCount, radius, radius2, 0);

            var points = poly.Flatten()[0].Points;

            // calcualte baselineDistance
            var baseline = Vector2.Distance(points[0], points[1]);

            // all points are extact the same distance away from the center
            Assert.Equal(pointsCount * 2, points.Length);
            for (var i = 0; i < points.Length; i++)
            {
                var j = i - 1;
                if (j < 0)
                {
                    j += points.Length;
                }

                var actual = Vector2.Distance(points[i], points[j]);
                Assert.Equal(baseline, actual, 3);
                if (i % 2 == 1)
                {
                    Assert.Equal(radius, Vector2.Distance(Vector2.Zero, points[i]), 3);
                }
                else
                {
                    Assert.Equal(radius2, Vector2.Distance(Vector2.Zero, points[i]), 3);
                }
            }
        }

        [Fact]
        public void AngleChangesOnePointToStartAtThatPosition()
        {
            const double TwoPI = 2 * Math.PI;
            float radius = 10;
            float radius2 = 20;
            double anAngle = new Random().NextDouble() * TwoPI;

            var poly = new Star(Vector2.Zero, 3, radius, radius2, (float)anAngle);
            var points = poly.Flatten();

            var allAngles = points[0].Points.Select(b => Math.Atan2(b.Y, b.X))
                .Select(x => x < 0 ? x + TwoPI : x); // normalise it from +/- PI to 0 to TwoPI

            Assert.Contains(allAngles, a => Math.Abs(a - anAngle) > 0.000001);
        }

        [Fact]
        public void TriangleMissingIntersectionsDownCenter()
        {

            var poly = new SixLabors.Shapes.Star(50, 50, 3, 50, 30);
            var points = poly.FindIntersections(new Vector2(0, 50), new Vector2(100, 50)).ToArray();

            Assert.Equal(2, points.Length);
        }

        [Fact]
        public void ClippingCornerShouldReturn2Points()
        {
            var star = new Star(40, 40, 3, 10, 20);
            var points = star.FindIntersections(new Vector2(0, 30), new Vector2(100, 30)).ToArray();

            Assert.True(points.Length % 2 == 0, "Should have even number of intersection points");
        }
    }
}
