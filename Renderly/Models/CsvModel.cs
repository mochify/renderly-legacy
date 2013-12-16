using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CsvHelper;

namespace Renderly.Models
{
    /// <summary>
    /// Prototype model class that opens a backing data store (CSV for now)
    /// </summary>
    public class CsvModel
    {
        public CsvModel()
        {
        }

        public IEnumerable<TestCase> GetTestCases(string csvFile)
        {
            var reader = new StreamReader(csvFile);
            var csv = new CsvReader(reader);
            csv.Configuration.RegisterClassMap<TestCaseMap>();
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.IgnoreHeaderWhiteSpace = true;
            return csv.GetRecords<TestCase>();
        }
    }
}
