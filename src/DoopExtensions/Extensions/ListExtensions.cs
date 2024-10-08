﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DoopExtensions.Extensions
{
    public static class ListExtensions
    {
        private static readonly HashSet<Type> NumericTypes = GetNumericTypes();
        private static readonly HashSet<Type> DateTypes = GetDateTypes();
        private static readonly HashSet<Type> StringTypes = GetStringTypes();
        public const double NumCompareThreshold = double.Epsilon;

        public static bool DoListsAgreeOnOverlappingAttributes<T, U>(this IEnumerable<T> source, IEnumerable<U> target)
        {
            if (source == null || target == null)
            {
                return false;
            }

            var sourceList = source.ToList();
            var targetList = target.ToList();

            if (sourceList.Count != targetList.Count)
            {
                return false;
            }

            var comparer = CreateComparer<T, U>();
            return sourceList.Zip(targetList, comparer).All(x => x);
        }

        private static Func<T, U, bool> CreateComparer<T, U>()
        {
            var tType = typeof(T);
            var uType = typeof(U);

            var tParam = Expression.Parameter(tType, "t");
            var uParam = Expression.Parameter(uType, "u");

            var comparisons = new List<Expression>();

            foreach (var tProp in tType.GetProperties())
            {
                var uProp = uType.GetProperty(tProp.Name);
                if (uProp != null && AreTypesComparable(tProp.PropertyType, uProp.PropertyType))
                {
                    var tValue = Expression.Property(tParam, tProp);
                    var uValue = Expression.Property(uParam, uProp);

                    var comparison = CreateComparison(tValue, uValue);
                    comparisons.Add(comparison);
                }
            }

            var body = comparisons.Count > 0 ? comparisons.Aggregate(Expression.And) : Expression.Constant(true);

            return Expression.Lambda<Func<T, U, bool>>(body, tParam, uParam).Compile();
        }

        private static BinaryExpression CreateComparison(Expression left, Expression right)
        {
            var leftType = left.Type;
            var rightType = right.Type;

            if (leftType.IsNumericType() && rightType.IsNumericType())
            {
                var convertedLeft = Expression.Convert(left, typeof(double));
                var convertedRight = Expression.Convert(right, typeof(double));
                var subtraction = Expression.Subtract(convertedLeft, convertedRight);
                var abs = Expression.Call(typeof(Math), "Abs", null, subtraction);
                return Expression.LessThan(abs, Expression.Constant(NumCompareThreshold));
            }

            return Expression.Equal(left, right);
        }

        private static Type GetUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        private static bool AreTypesComparable(Type type1, Type type2)
        {
            // see if the types are generally comparable.
            if (type1 == type2)
                return true;

            var num = type1.IsNumericType() && type2.IsNumericType();
            var date = type1.IsDateType() && type2.IsDateType();
            var letters = type1.IsStringType() && type2.IsStringType();
            return num || date || letters;
        }

        private static HashSet<Type> GetNumericTypes()
        {
            var numericTypes = new HashSet<Type> { typeof(decimal) };

            foreach (var type in typeof(int).Assembly.GetExportedTypes())
            {
                if (type.IsPrimitive && type.IsValueType && !type.IsEnum)
                {
                    var typeCode = Type.GetTypeCode(type);
                    if (typeCode >= TypeCode.SByte && typeCode <= TypeCode.Decimal)
                    {
                        numericTypes.Add(type);
                    }
                }
            }

            return numericTypes;
        }

        private static HashSet<Type> GetDateTypes() => new HashSet<Type>
        {
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan)
        };

        private static HashSet<Type> GetStringTypes() => new HashSet<Type>
        {
            typeof(string),
            typeof(char)
        };

        private static bool IsNumericType(this Type t) => NumericTypes.Contains(GetUnderlyingType(t));

        private static bool IsDateType(this Type t) => DateTypes.Contains(GetUnderlyingType(t));

        private static bool IsStringType(this Type t) => StringTypes.Contains(GetUnderlyingType(t));

        public static HashSet<Type> NumberTypes => NumericTypes;
    }
}