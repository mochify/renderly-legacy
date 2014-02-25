using CsvHelper.Configuration;

namespace Renderly.Models.Csv
{
    /// <summary>
    /// This maps CSV columns to TestCase objects for the CSV files used
    /// as part of the CsvModel class.
    /// 
    /// Look at <a href="http://joshclose.github.io/CsvHelper/#mapping-fluent-class-mapping">Fluent Class Mapping</a>
    /// on the CsvHelper project.
    /// </summary>
    class TestCaseMap : CsvClassMap<TestCase>
    {
        public override void CreateMap()
        {
            Map(m => m.TestId).Name("Test Id");
            Map(m => m.Type).Name("Type");
            Map(m => m.SourceLocation).Name("Source Location");
            Map(m => m.ReferenceLocation).Name("Reference Location");
            Map(m => m.DateAdded).Name("Date Added");
            Map(m => m.DateModified).Name("Date Modified");
            Map(m => m.Release).Name("Release");
            Map(m => m.Description).Name("Description");
        }
    }
}
