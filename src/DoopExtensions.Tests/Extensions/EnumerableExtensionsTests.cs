using DoopExtensions.Extensions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DoopExtensions.Tests.Extensions
{
    internal class EnumerableExtensionTests
    {
        private IEnumerable<int> _intItems = [];
        private IEnumerable<string> _stringItems = [];
        private Mock<Func<int, Task>> _mockBody = new();

        [SetUp]
        public void Setup()
        {
            _intItems = [1, 2, 3];
            _stringItems = ["a", "b", "c", "d"];
        }

        [Test]
        public void CanCount_Int()
        {
            GetCount<int>(); // signed
            GetCount<uint>(); // unsigned
        }

        [Test]
        public void CanCount_Long()
        {
            GetCount<long>();
            GetCount<ulong>();
        }

        [Test]
        public void CanCount_Short()
        {
            GetCount<short>();
            GetCount<ushort>();
        }

        [Test]
        public void CanCount_Byte()
        {
            // this is very slow compared to the others
            GetCount<byte>(); // unsigned
            GetCount<sbyte>(); // signed
        }

        [Test]
        public void CanCount_Float()
        {
            GetCount<float>();
        }

        [Test]
        public void CanCount_Double()
        {
            GetCount<double>();
        }

        [Test]
        public void CanCount_Decimal()
        {
            GetCount<decimal>();
        }

        [Test]
        public void GetMostFrequentValue_ExpectedOutput()
        {
            var intList = new List<int> { 1, 2, 3, 4, 5, 5 };
            var stringList = new List<string> { "a", "a", "a", "b", "b" };

            var intMFV = intList.GetMostFrequentValue();
            Assert.IsInstanceOf<int>(intMFV);
            Assert.AreEqual(5, intMFV);

            var strMFV = stringList.GetMostFrequentValue();
            Assert.IsInstanceOf<string>(strMFV);
            Assert.AreEqual("a", strMFV);
        }

        [Test]
        public void GetMostFrequentValue_CanThrowCorrectly()
        {
            var intList = new List<int> { 1, 2, 3, 4, 4, 5, 5 }; // ties
            var emptyList = new List<string>(); // nothing
            var nullList = new List<int?> { null }; // null values that will be a key

            Assert.Throws<IndexOutOfRangeException>(() => intList.GetMostFrequentValue());
            Assert.Throws<ArgumentException>(() => emptyList.GetMostFrequentValue());
            Assert.Throws<ArgumentNullException>(() => nullList.GetMostFrequentValue());
        }

        [Test]
        public async Task AsyncParallelForEach_ProcessesItemsInParallel()
        {
            var items = Enumerable.Range(1, 10);
            var newList = new ConcurrentBag<int>();
            var mockBody = new Mock<Func<int, Task>>();
            mockBody.Setup(x => x(It.IsAny<int>())).Returns<int>(async number => await IncrementAsync(number, newList));
            await AsyncParallelForEach(items, mockBody.Object, maxDegreeOfParallelism: 2);
            mockBody.Verify(x => x(It.IsAny<int>()), Times.Exactly(10));
        }

        [Test]
        public async Task AsyncParallelForEach_ExecutesUnboundedWhenSpecified()
        {
            var items = Enumerable.Range(1, 10).ToList();
            var newList = new ConcurrentBag<int>();
            var mockBody = new Mock<Func<int, Task>>();
            mockBody.Setup(x => x(It.IsAny<int>())).Returns<int>(async number => await IncrementAsync(number, newList));
            await AsyncParallelForEach(items, mockBody.Object);
            mockBody.Verify(x => x(It.IsAny<int>()), Times.Exactly(10));

            Assert.AreEqual(items.Count, newList.Count);
            items.ForEach(x => Assert.Contains(x, newList));

            // this requires order to be the same
            // CollectionAssert.AreEqual(items, newList.ToList());
        }

        private static Task AsyncParallelForEach<T>(IEnumerable<T> source, Func<T, Task> body,
            int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded)
        {
            return source.AsyncParallelForEach(body, maxDegreeOfParallelism);
        }

        private static async Task IncrementAsync(int number, ConcurrentBag<int> newItems)
        {
            await Task.Delay(1);
            newItems.Add(number);
        }

        private void GetCount<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            var res = _intItems.GetCount<T>();
            Assert.IsInstanceOf<T>(res);
            Assert.AreEqual(_intItems.Count(), res);

            var res2 = _stringItems.GetCount<T>();
            Assert.IsInstanceOf<T>(res2);
            Assert.AreEqual(_stringItems.Count(), res2);
        }
    }
}