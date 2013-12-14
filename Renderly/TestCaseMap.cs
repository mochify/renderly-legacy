using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;

namespace Renderly
{
    class TestCaseMap : CsvClassMap<TestCase>
    {
        public override void CreateMap()
        {
            Map(m => m.TestId).Index(0);
            Map(m => m.Type).Index(1);
            Map(m => m.Url).Index(2);
            Map(m => m.ReferenceLocation).Index(3);
            Map(m => m.DateAdded).Index(4);
            Map(m => m.DateModified).Index(5);
            Map(m => m.Release).Index(6);
            Map(m => m.Description).Index(7);
        }
    }
}
