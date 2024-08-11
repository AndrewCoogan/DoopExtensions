using NUnit.Framework;
using System.Threading.Tasks;

namespace DoopExtensions.Tests.Extensions
{
    internal class SqlConnectionExtensionsTests
    {
        public SqlConnectionExtensionsTests()
        {
        }

        [Test]
        public async Task WillRetryATaskForSqlDeadlockException()
        {
        }
    }
}