﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers;

namespace UnitTests.HighPerformance.Buffers
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_SpanOwnerOfT
    {
        [TestCategory("SpanOwnerOfT")]
        [TestMethod]
        public void Test_SpanOwnerOfT_AllocateAndGetMemoryAndSpan()
        {
            using var buffer = SpanOwner<int>.Allocate(127);

            Assert.IsTrue(buffer.Length == 127);
            Assert.IsTrue(buffer.Span.Length == 127);

            buffer.Span.Fill(42);

            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
        }

        [TestCategory("SpanOwnerOfT")]
        [TestMethod]
        public void Test_SpanOwnerOfT_AllocateFromCustomPoolAndGetMemoryAndSpan()
        {
            var pool = new TrackingArrayPool<int>();

            using (var buffer = SpanOwner<int>.Allocate(127, pool))
            {
                Assert.AreEqual(pool.RentedArrays.Count, 1);

                Assert.IsTrue(buffer.Length == 127);
                Assert.IsTrue(buffer.Span.Length == 127);

                buffer.Span.Fill(42);

                Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
            }

            Assert.AreEqual(pool.RentedArrays.Count, 0);
        }

        [TestCategory("SpanOwnerOfT")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_SpanOwnerOfT_InvalidRequestedSize()
        {
            using var buffer = SpanOwner<int>.Allocate(-1);

            Assert.Fail("You shouldn't be here");
        }

        [TestCategory("HashCodeOfT")]
        [TestMethod]
        public void Test_SpanOwnerOfT_PooledBuffersAndClear()
        {
            using (var buffer = SpanOwner<int>.Allocate(127))
            {
                buffer.Span.Fill(42);
            }

            using (var buffer = SpanOwner<int>.Allocate(127))
            {
                Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
            }

            using (var buffer = SpanOwner<int>.Allocate(127, AllocationMode.Clear))
            {
                Assert.IsTrue(buffer.Span.ToArray().All(i => i == 0));
            }
        }
    }
}
