using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Renderly;
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
        private bool CopyReferenceImages { get; set; }
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
            Threshold = 1.0f;
            ReportName = string.Format("renderly-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);

            IsCommand("run", "Run a rendering job");
            HasRequiredOption("f|file=", "The model to get test cases from.", x => DataSource = x);
            HasRequiredOption("o|outdir=", "The directory to generate the report in", x => OutputDirectory = x);
            HasRequiredOption("m|templatedir=", "The directory to get templates for report generation", x => TemplateDirectory = x);
            HasOption("n|name=", "The name of the report to generate. Defaults to runtime of the app otherwise", x => ReportName = x);
            HasOption("threshold=", "Threshold value to configure how aggressive image comparison is (0-100). 100 is exact match. Defaults to 100.",
                x => Threshold = float.Parse(x) / 100.0f);
            HasOption("showall", "Show all results in report (including successes). By default, only failures are shown.",
                x => ReportAllResults = x != null);
            HasOption("copyref", "Copy reference images locally to report directory. Default false.",
                x => CopyReferenceImages = x != null);
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
            // save people from aggressive thresholding.
            if (Threshold > 100.0f)
            {
                Threshold = 1.0f;
            }

            var fileManager = new RenderlyNativeAssetManager();

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

                var reportConfiguration = new ReportServiceConfiguration();
                reportConfiguration.CopyReferenceImages = CopyReferenceImages;
                reportConfiguration.DisplaySuccesses = ReportAllResults;
                reportConfiguration.OutputDirectory = OutputDirectory;
                reportConfiguration.TemplateDirectory = TemplateDirectory;
                reportConfiguration.ReportName = ReportName;
                reportConfiguration.ReportView = new MustacheView();
                var reportService = new ReportService(fileManager, reportConfiguration);

                var controller = new RenderingController(new ExhaustiveTemplateComparer(Threshold), fileManager);
                var directory = Directory.CreateDirectory(OutputDirectory);
                var results = controller.RunTests(testCases);
                foreach (var r in results) using (r)
                {
                    reportService.AddResult(r);
                }

                reportService.GenerateReport();
            }
            return 0;
        }
    }
}
