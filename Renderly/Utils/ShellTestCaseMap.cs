using CsvHelper.Configuration;

namespace Renderly.Utils
{
    class ShellTestCaseMap : CsvClassMap<ShellTestCase>
    {
        public override void CreateMap()
        {
            Map(m => m.Type).Name("Type");
            Map(m => m.ReferenceLocation).Name("Reference Location");
            Map(m => m.ReferenceSavePath).Name("Reference Save Directory");
            Map(m => m.Release).Name("Release");
            Map(m => m.Description).Name("Description");
        }
    }
}
