using System.Collections.Generic;
using System.Linq;
using System.IO;
using CsvHelper;

namespace Renderly.Utils
{
    public class ShellTestCsvModel
    {
        private IList<ShellTestCase> _data;
        public ShellTestCsvModel(Stream csvStream)
        {
            using (var sr = new StreamReader(csvStream))
            using (var csv = new CsvReader(sr))
            {
                csv.Configuration.RegisterClassMap<ShellTestCaseMap>();
                csv.Configuration.HasHeaderRecord = true;

                _data = csv.GetRecords<ShellTestCase>().ToList();
            }
        }

        public IEnumerable<ShellTestCase> GetTestCases()
        {
            return _data.Skip(0);
        }
    }
}
