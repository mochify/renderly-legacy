using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;

namespace Renderly.Utils
{
    class ShellTestCaseMap : CsvClassMap<ShellTestCase>
    {
        public override void CreateMap()
        {
            Map(m => m.Type).Name("Type");
            Map(m => m.ReferenceLocation).Name("Reference Location");
            Map(m => m.ReferenceSaveFolder).Name("Reference Save Folder");
            Map(m => m.Release).Name("Release");
            Map(m => m.Description).Name("Description");
        }
    }
}
