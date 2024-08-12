using DoopExtensions.Extensions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DoopExtensions.Tests.Extensions
{
    internal class ListExtensionTests
    {
        private class SimpleClass
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public int ValueToCheck { get; set; }
        }

        private class DifferentClass
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public double ValueToCheck { get; set; }
        }

        private class NumericClass
        {
            public int IntValue { get; set; }
            public double DoubleValue { get; set; }
        }

        private class DynamicClass<T>
        {
            public DynamicClass(T value) => Value = value;

            public T? Value { get; set; }
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
        public void DoListsAgreeOnOverlappingAttributes_DifferentClassDifferentTypes()
        {
            var list1 = new List<SimpleClass> { new() { Id = 1, ValueToCheck = 5 } };
            var list2 = new List<DifferentClass> { new() { Id = 1, ValueToCheck = 5 } };
            Assert.IsTrue(list1.DoListsAgreeOnOverlappingAttributes(list2));
        }

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_DifferentNumerics()
        {
            var list1 = new List<DynamicClass<int>> { new(1) };
            var list2 = new List<DynamicClass<double>> { new(1) };
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

        [Test]
        public void DoListsAgreeOnOverlappingAttributes_TestALotOfDifferentNumberTypes()
        {
            var numTypes = ListExtensions.NumberTypes;
            var testedPairs = new Dictionary<string, bool>();
            foreach (var type1 in numTypes)
            {
                foreach (var type2 in numTypes)
                {
                    // I am going to allow this to compare to itself.
                    ArgumentNullException.ThrowIfNull(type1);
                    ArgumentNullException.ThrowIfNull(type2);

                    var dynamicClassType1 = typeof(DynamicClass<>).MakeGenericType(type1);
                    var listType = typeof(List<>).MakeGenericType(dynamicClassType1);
                    var list1 = Activator.CreateInstance(listType) as IList;

                    var dynamicClassType2 = typeof(DynamicClass<>).MakeGenericType(type2);
                    var listType2 = typeof(List<>).MakeGenericType(dynamicClassType2);
                    var list2 = Activator.CreateInstance(listType2) as IList;

                    var dynamicClassCtor1 = dynamicClassType1.GetConstructor(new[] { type1 });
                    var dynamicClassCtor2 = dynamicClassType2.GetConstructor(new[] { type2 });

                    ArgumentNullException.ThrowIfNull(list1);
                    ArgumentNullException.ThrowIfNull(list2);
                    ArgumentNullException.ThrowIfNull(dynamicClassCtor1);
                    ArgumentNullException.ThrowIfNull(dynamicClassCtor2);
                    for (int i = 0; i < 5; i++)
                    {
                        var value1 = Convert.ChangeType(i, type1);
                        var value2 = Convert.ChangeType(i, type2);
                        list1.Add(dynamicClassCtor1.Invoke(new[] { value1 }));
                        list2.Add(dynamicClassCtor2.Invoke(new[] { value2 }));
                    }

                    // Now you can call your method using reflection
                    var methodInfo = typeof(ListExtensions).GetMethod("DoListsAgreeOnOverlappingAttributes");
                    ArgumentNullException.ThrowIfNull(methodInfo);
                    var genericMethod = methodInfo.MakeGenericMethod(dynamicClassType1, dynamicClassType2);
                    var result = (bool)genericMethod.Invoke(null, new[] { list1, list2 });

                    testedPairs.Add($"{type1}_{type2}", result);
                    Assert.IsTrue(result, $"Failed for types {type1.Name} and {type2.Name}");
                }
            }

            Assert.IsTrue(testedPairs.All(v => v.Value));
        }
    }
}