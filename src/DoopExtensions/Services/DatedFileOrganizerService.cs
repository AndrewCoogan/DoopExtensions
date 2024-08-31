using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DoopExtensions.Services
{
    public static class DatedFileOrganizer
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">source filepath</param>
        /// <returns></returns>
        public static async Task DataMigratorAsync<T>(string filePath)
        {
        }

        public static async Task<bool> MoveFile(string fileName)
        {
            return true;

        }
    }
}