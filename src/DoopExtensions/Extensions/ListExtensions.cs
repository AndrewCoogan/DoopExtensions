using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DoopExtensions.Extensions
{
    public static class ListExtensions
    {
        private static readonly HashSet<Type> NumericTypes = GetNumericTypes();

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

            var body = comparisons.Count > 0
                ? comparisons.Aggregate(Expression.And)
                : Expression.Constant(true);

            return Expression.Lambda<Func<T, U, bool>>(body, tParam, uParam).Compile();
        }

        private static Expression CreateComparison(Expression left, Expression right)
        {
            var leftType = left.Type;
            var rightType = right.Type;

            if (leftType.IsNumericType() && rightType.IsNumericType())
            {
                var convertedLeft = Expression.Convert(left, typeof(double));
                var convertedRight = Expression.Convert(right, typeof(double));
                var subtraction = Expression.Subtract(convertedLeft, convertedRight);
                var abs = Expression.Call(typeof(Math), "Abs", null, subtraction);
                return Expression.LessThan(abs, Expression.Constant(double.Epsilon));
            }

            return Expression.Equal(left, right);
        }

        private static Type GetUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        private static HashSet<Type> GetNumericTypes()
        {
            var numericTypes = new HashSet<Type>();

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

            numericTypes.Add(typeof(decimal));
            return numericTypes;
        }

        private static bool AreTypesComparable(Type type1, Type type2)
        {
            // see if the types are generally comparable.
            if (type1 == type2)
                return true;

            return type1.IsNumericType() && type2.IsNumericType();
        }

        private static bool IsNumericType(this Type t)
        {
            return NumericTypes.Contains(GetUnderlyingType(t));
        }
    }
}