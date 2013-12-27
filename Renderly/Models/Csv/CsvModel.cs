using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MoreLinq;

using CsvHelper;
using Renderly.Utils;

namespace Renderly.Models.Csv
{
    /// <summary>
    /// Prototype model class that opens a backing data store (CSV for now)
    /// </summary>
    public class CsvModel : ITestCaseModel, IDisposable
    {
        private bool _disposed;
        private IList<TestCase> _data;
        private Stream _csvStream;
        private IRenderlyAssetManager _fileManager;

        /// <summary>
        /// Create a CsvModel with a stream to read and write to.
        /// </summary>
        /// <param name="csvStream">A stream that contains CSV data to into the model.</param>
        public CsvModel(Stream csvStream, IRenderlyAssetManager fileManager)
        {
            _fileManager = fileManager;
            _csvStream = csvStream;
            _csvStream.Position = 0;

            // We do not want to dispose of the underlying stream, which
            // is what StreamReader and CsvReader do if you wrap it in a 'using' block
            var sr = new StreamReader(_csvStream);

            // If our stream was empty before we got it, initialize data to empty
            if (!sr.EndOfStream)
            {
                var csv = new CsvReader(sr);
                csv.Configuration.RegisterClassMap<TestCaseMap>();
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.IsHeaderCaseSensitive = false;
                _data = csv.GetRecords<TestCase>().ToList();
            }
            else
            {
                _data = new List<TestCase>();
            }
        }

        public IEnumerable<TestCase> GetTestCases()
        {
            // If you're unfamiliar with this Skip(0), it basically yields
            // each item in the list but you can not use the returned IEnumerable
            // to modify the underlying collection by casting it back to ICollection
            return _data.Skip(0);
        }

        public IEnumerable<TestCase> GetTestCases(Func<TestCase, bool> predicate)
        {
            return _data.Where(predicate);
        }

        /// <summary>
        /// Adds a test case to the model. Since this is CSV, the Test ID field will be mutated automatically by this to the
        /// next highest-available test case ID. Does not use gaps (i.e. if you have test case IDs 1 and 3, this does not use '2'
        /// for the added case. Instead it will use '4').
        /// </summary>
        /// <param name="tc"></param>
        public void AddTestCase(TestCase tc)
        {
            tc.TestId = NextTestId;
            InternalAddTestCase(tc);
        }

        public void AddTestCases(IEnumerable<TestCase> it)
        {
            // TODO this stifles multi-threading/parallel methods doing AddTestCases/AddTestCase
            // since this method currently assumes it has full control over the Test ID generation.
            // Of course, if you don't care about having distinct Test IDs in your CSV or will
            // fix them later, then parallelize away.
            var maxTestId = NextTestId;
            foreach (var t in it)
            {
                t.TestId = maxTestId;
                AddTestCase(t);
                ++maxTestId;
            }
        }

        private void InternalAddTestCase(TestCase tc)
        {
            _data.Add(tc);
        }

        private int NextTestId
        {
            get { return _data.Any() ? (_data.MaxBy(t => t.TestId)).TestId + 1 : 1; }
        }

        /// <summary>
        /// Delete test cases from the model.
        /// </summary>
        /// <param name="predicate">A predicate lambda/delegate that evaluates to true
        /// and is used to determine what items to delete.</param>
        /// <returns>A count of all the uploads removed from the underlying model</returns>
        public int Delete(Func<TestCase, bool> predicate)
        {
            var beforeCount = _data.Count();
            var toDelete = _data.Where(predicate).ToList();
            foreach (var tc in toDelete)
            {
                _fileManager.Delete(tc.ReferenceLocation);
            }
            _data = _data.Where(x => !predicate(x)).ToList();
            return beforeCount - _data.Count();
        }


        /// <summary>
        /// Write out the CSV file again.
        /// </summary>
        public void Save()
        {
            // we do not want StreamWriter or CsvWriter to close the stream,
            // so do not wrap this in using {} or close() it.
            _csvStream.Position = 0;
            var sw = new StreamWriter(_csvStream);
            var csv = new CsvWriter(sw);
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<TestCaseMap>();
            csv.WriteHeader(typeof(TestCase));
            csv.WriteRecords(GetTestCases());
            // have to manually flush because we're not wrapping the writer in a 'using'
            sw.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_csvStream != null)
                {
                    _csvStream.Dispose();
                }
            }

            _disposed = true;
            _csvStream = null;
        }
    }
}
