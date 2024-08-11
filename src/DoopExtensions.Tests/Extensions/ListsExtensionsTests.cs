using DoopExtensions.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace DoopExtensions.Tests.Extensions
{
    internal class SqlConnectionExtensionsTests
    {
        private class SimpleClass
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        private class DifferentClass
        {
            public int Id { get; set; }
            public string? Title { get; set; }
        }

        private class NumericClass
        {
            public int IntValue { get; set; }
            public double DoubleValue { get; set; }
        }

        private class DynamicClass<T>
        {
            public required T Value { get; set; }
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_SimpleMatch()
        {
            var list1 = new List<SimpleClass> { new() { Id = 1, Name = "Test" } };
            var list2 = new List<SimpleClass> { new() { Id = 1, Name = "Test" } };
            Assert.IsTrue(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_SimpleMismatch()
        {
            var list1 = new List<SimpleClass> { new() { Id = 1, Name = "Test" } };
            var list2 = new List<SimpleClass> { new() { Id = 2, Name = "Test" } };
            Assert.IsFalse(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }


        [Test]
        public void DoListsAgreeOnOverlappingAttributes_OrderInvariance()
        {
            var item1 = new SimpleClass { Id = 1, Name = "1" };
            var item12 = new SimpleClass { Id = 1, Name = "1" };
            var item2 = new SimpleClass { Id = 2, Name = "2" };
            var item22 = new SimpleClass { Id = 2, Name = "2" };

            var list1 = new List<SimpleClass> { item1, item2 };
            var list2 = new List<SimpleClass> { item22, item12 };
            Assert.IsFalse(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_DifferentClass()
        {
            var list1 = new List<SimpleClass> { new() { Id = 1, Name = "Shouldn't Matter" } };
            var list2 = new List<DifferentClass> { new() { Id = 1, Title = "Doesn't Matter" } };
            Assert.IsTrue(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_DifferentNumerics()
        {
            var list1 = new List<DynamicClass<int>> { new() { Value = 1 } };
            var list2 = new List<DynamicClass<double>> { new() { Value = 1 } };
            Assert.IsTrue(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_NumericComparison()
        {
            var list1 = new List<NumericClass> { new() { IntValue = 1, DoubleValue = 1.0 } };
            var list2 = new List<NumericClass> { new() { IntValue = 1, DoubleValue = 1.0 + double.Epsilon / 2 } };
            Assert.IsTrue(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_EmptyLists()
        {
            var list1 = new List<SimpleClass>();
            var list2 = new List<SimpleClass>();
            Assert.IsTrue(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }
    }
}