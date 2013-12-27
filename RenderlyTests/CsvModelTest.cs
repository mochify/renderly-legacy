using System;
using System.Linq;
using System.IO;
using NUnit.Framework;

using Renderly;
using Renderly.Models.Csv;
using RenderlyTests.Mocks;

namespace RenderlyTests
{
    [TestFixture]
    public class CsvModelTest
    {
        private string[] _header = { "Test Id", "Type", "Source Location", 
                "Reference Location", "Date Added", "Date Modified", "Release", "Description" };

        private string[] _records =  new string[]
        {
            "1,lp,source,reference,12/24/2013,12/25/13,27.5,Test Case",
            "2,shape,source2,reference2,11/12/2011,11/12/2011,27.7,Case 2",
            "3,type3,source3,reference3,10/10/2010,10/10/10,27.5,Case 3",
            "250,type4,source4,ref4,12/25/2014,01/01/2015,27.5,Case4",
            "127,type5,source5,ref5,12/12/2009,12/12/2010,27.8,Case5"
        };

        [Test]
        public void TestEmptyConstruction()
        {
            var stream = new MemoryStream();
            var manager = new MockFileManager();
            using (var model = new CsvModel(stream, manager))
            {
                var testCases = model.GetTestCases();
                Assert.AreEqual(0, testCases.Count());
            }
        }

        [Test]
        public void TestConstruction()
        {
            var stream = new MemoryStream();
            var manager = new MockFileManager();
            using (var writer = new StreamWriter(stream))
            {
                var record = _records[0].Split(',');
                writer.WriteLine(string.Join(",", _header));
                writer.WriteLine(string.Join(",", record));
                writer.Flush();
                stream.Position = 0;

                using (var model = new CsvModel(stream, manager))
                {
                    var testCases = model.GetTestCases().ToList();
                    Assert.AreEqual(1, testCases.Count);

                    var tc = testCases.First();
                    Assert.AreEqual(Int32.Parse(record[0]), tc.TestId);
                    Assert.AreEqual(record[1], tc.Type);
                    Assert.AreEqual(record[2], tc.SourceLocation);
                    Assert.AreEqual(record[3], tc.ReferenceLocation);
                    Assert.AreEqual(DateTime.Parse(record[4]).Date, tc.DateAdded.Date);
                    Assert.AreEqual(DateTime.Parse(record[5]).Date, tc.DateModified.Date);
                    Assert.AreEqual(record[6], tc.Release);
                    Assert.AreEqual(record[7], tc.Description);
                }
            }
        }

        [Test]
        public void TestDeleteQuery()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(string.Join(",", _header));
                
                foreach (var r in _records)
                {
                    writer.WriteLine(r);
                }
                writer.Flush();
                stream.Position = 0;

                var manager = new MockFileManager();

                using (var model = new CsvModel(stream, manager))
                {
                    var count = model.Delete(x => x.SourceLocation == "source6");
                    Assert.AreEqual(0, count);

                    var deleted = model.Delete(x => x.TestId == 1);
                    Assert.AreEqual(1, deleted);
                    Assert.AreEqual(_records.Length - deleted, model.GetTestCases().Count());

                    var deleted2 = model.Delete(x => (x.Release == "27.8" || x.ReferenceLocation == "ref4"));
                    // previous should have deleted two items if our rows are correct
                    Assert.AreEqual(2, deleted2);

                    var deleted3 = model.Delete(x => x.TestId > 0);
                    Assert.AreEqual(0, model.GetTestCases().Count());
                    
                }
            }
        }

        [Test]
        public void TestGetTestCases()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(string.Join(",", _header));

                foreach (var r in _records)
                {
                    writer.WriteLine(r);
                }
                writer.Flush();
                stream.Position = 0;

                var manager = new MockFileManager();
                using (var model = new CsvModel(stream, manager))
                {
                    // get everything
                    var testcases = model.GetTestCases().ToList();
                    Assert.AreEqual(_records.Length, testcases.Count());
                    

                    // selective querying
                    var testcases2 = model.GetTestCases(x => x.TestId == 1).ToList();
                    Assert.AreEqual(1, testcases2.Count());
                }
            }
        }

        [Test]
        public void TestAddTestCase()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(string.Join(",", _header));

                foreach (var r in _records)
                {
                    writer.WriteLine(r);
                }
                writer.Flush();
                stream.Position = 0;

                var manager = new MockFileManager();
                using (var model = new CsvModel(stream, manager))
                {
                    var newCase = new TestCase
                    {
                        TestId = 1000,
                        Type = "Newtype",
                        Release = "28.6"
                    };
                    model.AddTestCase(newCase);
                    Assert.AreEqual(1 + _records.Length, model.GetTestCases().Count());
                    var cases = model.GetTestCases(x => x.TestId == 251).ToList();
                    Assert.AreEqual(1, cases.Count());
                    Assert.AreEqual("Newtype", cases.First().Type);

                    // This tests that the ID generator is correct in the CsvModel
                    Assert.AreEqual(251, cases.First().TestId);
                }
            }
        }

        [Test]
        public void TestAddTestCaseProducesCorrectId()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(string.Join(",", _header));
                writer.Flush();

                var manager = new MockFileManager();
                using (var model = new CsvModel(stream, manager))
                {
                    var newCase = new TestCase
                    {
                        TestId = 1000,
                        Type = "Newtype",
                        Release = "28.6"
                    };
                    model.AddTestCase(newCase);
                    var cases = model.GetTestCases(x => x.TestId == 1000).ToList();
                    // this should not have found anything because the CSV Model will
                    // assume its own ID generation responsibility.
                    Assert.AreEqual(0, cases.Count());

                    var allCases = model.GetTestCases();
                    Assert.AreEqual(1, allCases.Count());
                    Assert.AreEqual(1, allCases.First().TestId);
                }
            }
        }

        [Test]
        public void TestSave()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            {
                // write the header, open the model, add a test case, save out.
                writer.WriteLine(string.Join(",", _header));
                writer.Flush();
                stream.Position = 0;

                var manager = new MockFileManager();
                using (var model = new CsvModel(stream, manager))
                {
                    var newCase = new TestCase
                    {
                        TestId = 1000,
                        Type = "Newtype",
                        Release = "28.6"
                    };
                    model.AddTestCase(newCase);
                    var testCases = model.GetTestCases();
                    Assert.AreEqual(1, testCases.Count());
                    model.Save();
                    // check the stream by resetting its position to 0 and then stream reading.
                    stream.Position = 0;
                    var headerLine = reader.ReadLine();
                    Assert.AreEqual(string.Join(",", _header), headerLine);
                    var next = reader.ReadLine();
                    var defaultDate = new DateTime();

                    // Because the test cases were empty to start,
                    // the ID generator in the CSV Model should have started at ID 1
                    Assert.AreEqual(string.Format("1,Newtype,,,{0},{0},28.6,", defaultDate), next);

                    // stream should now be empty
                    Assert.IsTrue(reader.EndOfStream);
                }
            }
        }

        [Test]
        public void TestEmptySave()
        {
            var stream = new MemoryStream();
            var manager = new MockFileManager();
            using (var reader = new StreamReader(stream))
            using (var model = new CsvModel(stream, manager))
            {
                model.Save();
                stream.Position = 0;
                var headerLine = reader.ReadLine();
                Assert.AreEqual(string.Join(",", _header), headerLine);

                // file should be empty now as the header should be the only
                // thing written.
                Assert.IsTrue(reader.EndOfStream);
            }
            
        }

        [Test]
        public void TestDispose()
        {
            var stream = new MemoryStream();
            var manager = new MockFileManager();
            using (var writer = new StreamWriter(stream))
            {
                using (var model = new CsvModel(stream, manager))
                {
                    // During the lifetime of the CsvModel,
                    // this stream should be writeable
                    Assert.IsTrue(stream.CanWrite);
                    Assert.IsTrue(stream.CanRead);
                }

                // Collapsing the 'using' block should have done model.Dispose
                // and released the stream it used.
                Assert.IsFalse(stream.CanWrite);
                Assert.IsFalse(stream.CanRead);
            }
        }
    }
}
