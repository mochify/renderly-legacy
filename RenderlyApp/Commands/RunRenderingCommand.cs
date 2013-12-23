using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Renderly.Models;
using Renderly.Controllers;
using Renderly.Imaging;
using Renderly.Views;

using ManyConsole;


namespace RenderlyApp.Commands
{
    class RunRenderingCommand : ConsoleCommand
    {
        public string DataSource { get; set; }
        public string ReportName { get; set; }
        public string OutputDirectory { get; set; }
        public string TemplateDirectory { get; set; }
        private IEnumerable<DateTime> Dates { get; set; }
        private IEnumerable<string> Releases { get; set; }
        private IEnumerable<int> TestIds { get; set; }


        public RunRenderingCommand()
        {
            Dates = Enumerable.Empty<DateTime>();
            Releases = Enumerable.Empty<string>();
            TestIds = Enumerable.Empty<int>();

            IsCommand("run", "Run a rendering job");
            HasRequiredOption("f|file=", "The model to get test cases from.", x => DataSource = x);
            HasRequiredOption("n|name=", "The name of the report to generate", x => ReportName = x);
            HasRequiredOption("o|outdir=", "The directory to generate the report in", x => OutputDirectory = x);
            HasRequiredOption("m|templatedir=", "The directory to get templates for report generation", x => TemplateDirectory = x);
            HasOption("t|testids=", "Comma-separated list of test IDs to run",
                x => { TestIds = x.Split(',').Select(Int32.Parse); });
            HasOption("r|releases=", "Comma-separated list of releases to run",
                x => { Releases = x.Split(','); });
            HasOption("d|dates=", "Comma-separated list of dates to run. MM-DD-YYYY format. Runs for Date Added.",
                x => { Dates = x.Split(',').Select(DateTime.Parse); });
        }

        public override int Run(string[] remainingArguments)
        {
            var model = new CsvModel(DataSource);

            IEnumerable<TestCase> testCases;

            if (!Dates.Any() && !Releases.Any() && !TestIds.Any())
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

                foreach(var t in TestIds)
                {
                    predicate = predicate.Or(x => x.TestId == t);
                }

                testCases = model.GetTestCases(predicate.Compile());
            }
            
            var controller = new RenderingController(new StandaloneImageComparator());
            var directory = Directory.CreateDirectory(OutputDirectory);
            var reportDir = directory.CreateSubdirectory(ReportName);
            var results = controller.RunTests(testCases, reportDir);
            var reportDict = new Dictionary<string, object>();
            reportDict.Add("reportname", ReportName);
            reportDict.Add("result", results);
            var view = new View();
            var templateName = "rendering-results.mustache";
            var path = Path.Combine(TemplateDirectory, templateName);
            var html = view.GenerateReport(path, reportDict);
            using (var writer = new StreamWriter(Path.Combine(reportDir.FullName, "report.html")))
            {
                writer.WriteAsync(html);
            }
            return 0;
        }
    }
}
