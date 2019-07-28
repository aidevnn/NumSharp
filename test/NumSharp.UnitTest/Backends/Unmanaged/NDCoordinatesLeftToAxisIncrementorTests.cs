﻿using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumSharp.Backends;
using NumSharp.Backends.Unmanaged;
using NumSharp.Utilities;

namespace NumSharp.UnitTest.Backends.Unmanaged
{
    [TestClass]
    public class NDCoordinatesLeftToAxisIncrementorTests : TestClass
    {
        [TestMethod]
        public void Case1_Axis0()
        {
            var shape = new Shape(2, 3, 3);
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 0)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Case1_Axis1()
        {
            var shape = new Shape(2, 3, 3);
            var sh = new NDCoordinatesLeftToAxisIncrementor(ref shape, 1);
            var slices = sh.Slices;

            slices.Should().ContainInOrder(Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(0), Slice.Index(1), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(0), Slice.Index(2), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(1), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(1), Slice.Index(1), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(1), Slice.Index(2), Slice.All);
            sh.Next().Should().BeNull();
        }

        [TestMethod]
        public void Case1_Axis2()
        {
            var shape = new Shape(2, 3, 3);
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 2)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Case2_Axis2_OutOf_5()
        {
            var shape = new Shape(1, 2, 1, 1, 3);
            var sh = new NDCoordinatesLeftToAxisIncrementor(ref shape, 2);
            var slices = sh.Slices;

            slices.Should().ContainInOrder(Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.All, Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(0), Slice.Index(1), Slice.Index(0), Slice.All, Slice.All);
            sh.Next().Should().BeNull();
        }

        [TestMethod]
        public void Case2_Axis3_OutOf_5()
        {
            var shape = new Shape(1, 2, 1, 1, 3);
            var sh = new NDCoordinatesLeftToAxisIncrementor(ref shape, 3);
            var slices = sh.Slices;

            slices.Should().ContainInOrder(Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(0), Slice.Index(1), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().BeNull();
        }

        [TestMethod]
        public void Case5_Axis3_OutOf_5()
        {
            var shape = new Shape(3, 2, 1, 1, 3);
            var sh = new NDCoordinatesLeftToAxisIncrementor(ref shape, 3);
            var slices = sh.Slices;

            slices.Should().ContainInOrder(Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(0), Slice.Index(1), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(1), Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(1), Slice.Index(1), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(2), Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(2), Slice.Index(1), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().BeNull();
        }

        [TestMethod]
        public void Case2_Axis3()
        {
            var shape = new Shape(1, 2, 1, 1, 3);
            var sh = new NDCoordinatesLeftToAxisIncrementor(ref shape, 3);
            var slices = sh.Slices;

            slices.Should().ContainInOrder(Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().ContainInOrder(Slice.Index(0), Slice.Index(1), Slice.Index(0), Slice.Index(0), Slice.All);
            sh.Next().Should().BeNull();
        }

        [TestMethod]
        public void Case2_Axis4()
        {
            var shape = new Shape(1, 2, 1, 1, 3);
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 4)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Case3_Empty()
        {
            var shape = new Shape(0);
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 0)).Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Case4()
        {
            var shape = new Shape(1);
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 0)).Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Case5()
        {
            var shape = new Shape(2);
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 0)).Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Case6_Scalar()
        {
            var shape = Shape.Scalar;
            new Action(() => new NDCoordinatesLeftToAxisIncrementor(ref shape, 0)).Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
