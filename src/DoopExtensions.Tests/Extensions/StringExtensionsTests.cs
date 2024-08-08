using NUnit.Framework;

namespace DoopExtensions.Tests.Extensions
{
    internal class StringExtensionsTests
    {
        private string? sqlInjectDrop;
        private string? sqlTempTableCreationFromVarchar;
        private string? sqlTempTableCreationFromInt;

        [SetUp]
        public void Setup()
        {
            sqlInjectDrop = "--DROP table";
            sqlTempTableCreationFromVarchar = "varchar(255)";
            sqlTempTableCreationFromInt = "int";
        }

        [Test]
        public void CanDetectSQLInjectionForTempTable()
        {
        }
    }
}