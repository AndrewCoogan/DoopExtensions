using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoopExtensions.Services
{
    public static class DataMigrationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableSchema">Table schema (i.e. dbo)</param>
        /// <param name="tableName">Table name</param>
        /// <returns></returns>
        public static async Task DataMigratorAsync<T>(string tableSchema, string tableName)
        {
        }

        public static async Task<int> NumberToMove(string query)
        {
            Task.Delay(10).Wait();
            return 0;
        }
    }
}