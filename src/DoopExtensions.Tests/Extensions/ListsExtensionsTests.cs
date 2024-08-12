using DoopExtensions.Extensions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                ArgumentNullException.ThrowIfNull(type1);
                var (class1, list1, constructor1) = GenerateListStuff(type1);

                foreach (var type2 in numTypes)
                {
                    // I am going to allow this to compare to itself.
                    ArgumentNullException.ThrowIfNull(type2);
                    var (class2, list2, constructor2) = GenerateListStuff(type2);

                    for (int i = 0; i < 5; i++)
                    {
                        var value1 = Convert.ChangeType(i, type1);
                        var value2 = Convert.ChangeType(i, type2);
                        list1.Add(constructor1.Invoke(new[] { value1 }));
                        list2.Add(constructor2.Invoke(new[] { value2 }));
                    }

                    // Now you can call your method using reflection
                    var methodInfo = typeof(ListExtensions).GetMethod("DoListsAgreeOnOverlappingAttributes");
                    ArgumentNullException.ThrowIfNull(methodInfo);
                    var genericMethod = methodInfo.MakeGenericMethod(class1, class2);
                    var result = (bool)genericMethod.Invoke(null, new[] { list1, list2 });

                    testedPairs.Add($"{type1}_{type2}", result);
                    Assert.IsTrue(result, $"Failed for types {type1.Name} and {type2.Name}");

                    list1.Clear();
                }
            }

            Assert.IsTrue(testedPairs.All(v => v.Value));
        }

        private static (Type type, IList list, ConstructorInfo constructor) GenerateListStuff(Type inp)
        {
            // make the instance of the dynamic class
            ArgumentNullException.ThrowIfNull(inp);

            var type = typeof(DynamicClass<>).MakeGenericType(inp);
            ArgumentNullException.ThrowIfNull(type);

            // make the instance of the list
            var listType = typeof(List<>).MakeGenericType(type);
            var list = Activator.CreateInstance(listType) as IList;
            ArgumentNullException.ThrowIfNull(list);

            // make a list interface adding values to
            var constructor = type.GetConstructor(new[] { inp });
            ArgumentNullException.ThrowIfNull(constructor);
            return (type, list, constructor);
        }
    }
}