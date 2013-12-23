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


namespace RenderlyApp.Options
{
    class RunRenderingCommand : ConsoleCommand
    {
        public string DataSource { get; set; }
        public string ReportName { get; set; }
        public string OutputDirectory { get; set; }
        public string TemplateDirectory { get; set; }


        public RunRenderingCommand()
        {
            IsCommand("run", "Run a rendering job");
            HasRequiredOption("d|datasource=", "The datasource to get test cases from.", x => DataSource = x);
            HasRequiredOption("n|name=", "The name of the report to generate", x => ReportName = x);
            HasRequiredOption("o|outdir=", "The directory to generate the report in", x => OutputDirectory = x);
            HasRequiredOption("t|templatedir=", "The directory to get templates for report generation", x => TemplateDirectory = x);
        }

        public override int Run(string[] remainingArguments)
        {
            var testCases = new CsvModel(DataSource).GetTestCases();
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
