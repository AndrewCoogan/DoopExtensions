using System;

namespace DoopExtensions.Extensions
{
    internal static class TypeExtensions
    {
        internal static object GetDefault(this Type type)
        {
            return type.IsValueType ? (!type.IsGenericType ? Activator.CreateInstance(type) : type.GenericTypeArguments[0].GetDefault()) : null;
        }
    }
}