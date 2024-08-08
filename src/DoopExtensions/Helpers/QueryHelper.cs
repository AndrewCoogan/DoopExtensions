using DoopExtensions.Extensions;
using System.Reflection;

namespace DoopExtensions.Helpers
{
    internal class QueryHelper
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        internal static class TableSchema
        {
            internal static readonly string Select = _assembly.GetQuery("DoopExtensions.Queries.TableSchema.sql");
        }

        internal static class TempTableSchema
        {
            internal static readonly string Select = _assembly.GetQuery("DoopExtensions.Queries.TempTableSchema.sql");
        }
    }
}