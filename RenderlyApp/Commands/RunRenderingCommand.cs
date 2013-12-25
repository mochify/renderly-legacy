using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Renderly;
using Renderly.Models;
using Renderly.Models.Csv;
using Renderly.Controllers;
using Renderly.Imaging;
using Renderly.Reporting;
using Renderly.Utils;

using ManyConsole;


namespace RenderlyApp.Commands
{
    class RunRenderingCommand : ConsoleCommand
    {
        private string DataSource { get; set; }
        private string ReportName { get; set; }
        private string OutputDirectory { get; set; }
        private string TemplateDirectory { get; set; }
        private bool ReportAllResults { get; set; }
        private float Threshold { get; set; }
        private IEnumerable<DateTime> Dates { get; set; }
        private IEnumerable<string> Releases { get; set; }
        private IEnumerable<int> TestIds { get; set; }
        private IEnumerable<string> TestTypes { get; set; }

        public RunRenderingCommand()
        {
            Dates = Enumerable.Empty<DateTime>();
            Releases = Enumerable.Empty<string>();
            TestIds = Enumerable.Empty<int>();
            TestTypes = Enumerable.Empty<string>();

            IsCommand("run", "Run a rendering job");
            HasRequiredOption("f|file=", "The model to get test cases from.", x => DataSource = x);
            HasRequiredOption("n|name=", "The name of the report to generate", x => ReportName = x);
            HasRequiredOption("o|outdir=", "The directory to generate the report in", x => OutputDirectory = x);
            HasRequiredOption("m|templatedir=", "The directory to get templates for report generation", x => TemplateDirectory = x);
            HasOption("threshold=", "Threshold value to configure how aggressive image comparison is (0-100). 100 is exact match.",
                x => Threshold = float.Parse(x) / 100.0f);
            HasOption("showall", "Show all results in report (including successes). By default, only failures are shown.",
                x => ReportAllResults = x != null);
            HasOption("t|testids=", "Comma-separated list of test IDs to run",
                x => { TestIds = x.Split(',').Select(Int32.Parse); });
            HasOption("r|releases=", "Comma-separated list of releases to run",
                x => { Releases = x.Replace(" ", "").Split(','); });
            HasOption("d|dates=", "Comma-separated list of dates to run. MM-DD-YYYY format. Runs for Date Added.",
                x => { Dates = x.Split(',').Select(DateTime.Parse); });
            HasOption("y|types=", "Comma-separated list of types to run.",
                x => { TestTypes = x.Replace(" ", "").Split(','); });
        }

        public override int Run(string[] remainingArguments)
        {
            var fileManager = new RenderlyNativeFileManager();

            var stream = new FileStream(DataSource, FileMode.Open, FileAccess.Read);
            using (var model = new CsvModel(stream, fileManager))
            {

                IEnumerable<TestCase> testCases;

                if (!Dates.Any() && !Releases.Any() && !TestIds.Any() && !TestTypes.Any())
                {
                    testCases = model.GetTestCases();
                }
                else
                {
                    var predicate = PredicateBuilder.False<TestCase>();
                    foreach (var d in Dates)
                    {
                        predicate = predicate.Or(x => x.DateAdded.Date == d.Date);
                    }

                    foreach (var r in Releases)
                    {
                        predicate = predicate.Or(x => x.Release == r);
                    }

                    foreach (var t in TestIds)
                    {
                        predicate = predicate.Or(x => x.TestId == t);
                    }

                    foreach (var t in TestTypes)
                    {
                        predicate = predicate.Or(x => x.Type == t);
                    }

                    testCases = model.GetTestCases(predicate.Compile());
                }

                var controller = new RenderingController(new ExhaustiveTemplateComparer(Threshold), fileManager);
                var directory = Directory.CreateDirectory(OutputDirectory);
                var reportDir = directory.CreateSubdirectory(ReportName);
                controller.RunTests(testCases, reportDir);
                var reportDict = new Dictionary<string, object>();

                reportDict.Add("reportname", ReportName);

                //var failures = results.Where(r => !r.TestPassed);

                //var failcsv = string.Join(",", failures.Select(x => x.TestId));
                //reportDict.Add("failures", failcsv);

                if (ReportAllResults)
                {
                    //reportDict.Add("result", results);
                }
                else
                {
                    //reportDict.Add("result", failures);
                }
                var view = new MustacheView();
                var templateName = "rendering-results.mustache";
                var path = Path.Combine(TemplateDirectory, templateName);
                var html = view.GenerateReport(path, reportDict);
                using (var writer = new StreamWriter(Path.Combine(reportDir.FullName, "report.html")))
                {
                    writer.WriteAsync(html);
                }
            }
            return 0;
        }
    }
}
