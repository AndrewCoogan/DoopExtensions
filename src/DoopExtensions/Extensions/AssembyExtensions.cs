using System;
using System.IO;
using System.Reflection;

namespace DoopExtensions.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetQuery(this Assembly assembly, string resourceName)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));
            return new StreamReader(assembly.GetManifestResourceStream(resourceName)).ReadToEnd();
        }
    }
}