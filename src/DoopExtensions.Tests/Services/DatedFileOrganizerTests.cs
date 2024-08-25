using System;
using NUnit.Framework;

namespace DoopExtensions.Tests.Services
{
    internal class DatedFileOrganizer
    {
        private static readonly DateTime _maxDate = new(2024, 8, 25);
        private static readonly DateTime _minDate = new(2010, 01, 01);
        private static readonly int _dateDelta = Convert.ToInt32(_maxDate.Subtract(_minDate).Days);
        private readonly Random _rng = new();


        [SetUp]
        public void Setup()
        {
            /// need to make a file name generator, guid_date.xx 
            /// user should need to pass in date format in someting that can extract the dates
            /// another parameter can be an ignore list, with file name string to not move.
            /// ill need to move what a file move looks like. taking in the parameter, adding it to a list where it would have gone.
            /// or maybe just make a class here that has a file name, its original full file path, and let the File.Mock mock
            /// populate a second file location, where it would have ended up. fill file path, same as the first param.
            /// will need to write a way to validate.
        }

        private string GenerateFileName(string dateFormat)
        {
            var header = Guid.NewGuid().ToString();

            return string.Empty;
        }


        [Test]
        public void CanGetAllQueries()
        {
        }
    }
}