using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoopExtensions.Services
{
    public static class BulkUploadService
    {
        /// <summary>
        /// Leverages reflection to map to the target table and SqlBulkCopy for ultra-fast bulk inserts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableSchema">Table schema (i.e. dbo)</param>
        /// <param name="tableName">Table name</param>
        /// <param name="connectionString">SQL connection string</param>
        /// <param name="entities">Data to upload</param>
        /// <param name="batchSize">Load data in batches of x</param>
        /// <param name="numberOfRetries">The maximum number of attempts to retry.</param>
        /// <param name="bulkCopyTimeout">The integer value of the Microsoft.Data.SqlClient.SqlBulkCopy.BulkCopyTimeout property. The default is 30 seconds. A value of 0 indicates no limit; the bulk copy will wait indefinitely.</param>
        /// <returns></returns>
        public static async Task BulkUploadAsync<T>(string tableSchema, string tableName, string connectionString, IEnumerable<T> entities, int batchSize = 1000, int numberOfRetries = 5, int bulkCopyTimeout = 30)
        {
        }
    }
}