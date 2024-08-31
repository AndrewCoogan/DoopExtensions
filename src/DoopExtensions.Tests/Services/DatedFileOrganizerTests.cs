using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using DoopExtensions.Models;

namespace DoopExtensions.Tests.Services
{
    internal class DatedFileOrganizer
    {
        private static DatedFileOrganizer _dfo;
        private static readonly DateTime _maxDate = new(2024, 8, 25);
        private static readonly DateTime _minDate = new(2010, 01, 01);
        private static readonly int _dateDelta = Convert.ToInt32(_maxDate.Subtract(_minDate).Days);
        private static readonly Random _rng = new();


        [SetUp]
        public void Setup()
        {
            _dfo = new DatedFileOrganizer();
            /// need to make a file name generator, guid_date.xx 
            /// user should need to pass in date format in someting that can extract the dates
            /// another parameter can be an ignore list, with file name string to not move.
            /// ill need to move what a file move looks like. taking in the parameter, adding it to a list where it would have gone.
            /// or maybe just make a class here that has a file name, its original full file path, and let the File.Mock mock
            /// populate a second file location, where it would have ended up. fill file path, same as the first param.
            /// will need to write a way to validate.
        }

        [Test]
        public void CanMoveAFile()
        {

        }

        [Test]
        public void CanGenerateSomeFileNames()
        {
            var iterations = 10;
            var dateFormat = "yyyyMMdd";
            var requiredFileNameHeader = "InitTest";
            var requiredFileNameTail = ".xx";

            var files = GenerateFiles(iterations, dateFormat, requiredFileNameHeader, requiredFileNameTail);
            Assert.IsNotNull(files);
            Assert.IsNotEmpty(files);
            Assert.AreEqual(iterations, files.Count());
            Assert.That(files, Has.All.Matches<FileInfo>(f => f.FileName.StartsWith(requiredFileNameHeader)));
            Assert.That(files, Has.All.Matches<FileInfo>(f => f.FileName.EndsWith(requiredFileNameTail)));
        }

        private static IEnumerable<FileInfo> GenerateFiles(int count,
            string dateFormat,
            string requiredFileNameHeader = "",
            string requiredFileNameTail = "")
        {
            return Enumerable.Range(0, count).Select(_ =>
            {
                var f = GenerateFileName(dateFormat, requiredFileNameHeader, requiredFileNameTail);
                return new FileInfo(f);
            });
        }

        private static string GenerateFileName(string dateFormat,
            string requiredFileNameHeader = "",
            string requiredFileNameTail = "")
        {
            var fileNameDelimiter = "_";
            var body = Guid.NewGuid().ToString();
            var date = GenerateRandomDate().ToString(dateFormat);
            return $"{requiredFileNameHeader}{fileNameDelimiter}{body}{fileNameDelimiter}{date}{requiredFileNameTail}";
        }

        private static DateTime GenerateRandomDate()
        {
            var delta = _rng.Next(_dateDelta);
            return _minDate.AddDays(delta);
        }
    }
}