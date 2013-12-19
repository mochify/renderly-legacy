using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MoreLinq;

using CsvHelper;
using Renderly.Utils;

using System.Net;
using Microsoft.Win32;

namespace Renderly.Models
{
    /// <summary>
    /// Prototype model class that opens a backing data store (CSV for now)
    /// </summary>
    public class CsvModel
    {
        private IList<TestCase> _data;
        private readonly string _originalCsvName;

        public CsvModel(string csvFile)
        {
            // TODO move this work out of the constructor, right now
            // this is the easiest way to do this stuff
            _originalCsvName = csvFile;
            var info = new FileInfo(csvFile);

            if (info.Exists && info.Length > 0)
            {
                using (var sr = new StreamReader(csvFile))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.RegisterClassMap<TestCaseMap>();
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.IsHeaderCaseSensitive = false;
                    _data = csv.GetRecords<TestCase>().ToList();
                }
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

        public void GenerateTestCases(IEnumerable<ShellTestCase> shellCases)
        {
            // TODO if this is a significant runtime turtle, may need to convert
            // to accepting a List or countable-collection interface so that
            // I can preallocate a list for eventual joining, rather than
            // adding each test case one by one.
            var maxTestId = _data.Any() ? (_data.MaxBy(t => t.TestId)).TestId + 1 : 1;
            foreach (var stc in shellCases)
            {
                var outputDir = Directory.CreateDirectory(stc.ReferenceSaveFolder);
                var u = new Uri(stc.ReferenceLocation);


                var fname = "";
                if (u.IsUnc || u.IsFile)
                {
                    fname = string.Format("{0}/{1}-reference{2}", outputDir.FullName, maxTestId, Path.GetExtension(u.AbsolutePath));
                    File.Copy(u.AbsolutePath, fname);
                }
                else
                {
                    var request = WebRequest.Create(u.OriginalString) as HttpWebRequest;
                    var rsp = request.GetResponse();
                    fname = Path.Combine(stc.ReferenceSaveFolder, string.Format("{0}-reference.jpg", maxTestId));

                    using (var rspStream = rsp.GetResponseStream())
                    using (var fstream = File.OpenWrite(fname))
                    {
                        rspStream.CopyTo(fstream);
                    }
                }

                var tc = new TestCase
                {
                    Type = stc.Type,
                    Description = stc.Description,
                    ReferenceLocation = fname,
                    Release = stc.Release,
                    TestId = maxTestId,
                    SourceLocation = stc.ReferenceLocation,
                    DateAdded = DateTime.Now,
                    DateModified = DateTime.Now
                };

                ++maxTestId;
                _data.Add(tc);
            }

        }

        private void AddTestCase(TestCase tc)
        {
            _data.Add(tc);
        }

        /// <summary>
        /// Write out the CSV file again. If you don't specify outfile, or give it a null/empty
        /// string value, it will default to using the original CSV file that
        /// was opened.
        /// </summary>
        /// <param name="outfile">The file to dump the CSV to.</param>
        public void Serialize(string outfile = "")
        {
            var filename = string.IsNullOrEmpty(outfile) ? _originalCsvName : outfile;
            using (var sw = new StreamWriter(filename))
            {
                var csv = new CsvWriter(sw);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestCaseMap>();
                //csv.WriteHeader(typeof(TestCase));
                csv.WriteRecords(GetTestCases());
            }
        }
    }
}
