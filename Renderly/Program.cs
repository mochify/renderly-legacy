using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CsvHelper;

namespace Renderly
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new OptionParser();
            try
            {
                parser.ParseArgs(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem parsing arguments");
                Console.WriteLine(e.Message);
                return;
            }

            // now we want to open the model, then pass the data to the controller,
            // then generate a report

            if (parser.ShowHelp)
            {
                parser.ShowHelpMessage();
                return;
            }

            bool initialized = true;
            if (string.IsNullOrEmpty(parser.Directory))
            {
                Console.WriteLine("Specify a directory");
                initialized = false;
            }

            if (string.IsNullOrEmpty(parser.ReportName))
            {
                Console.WriteLine("Specify a report name");
                initialized = false;
            }

            if(!initialized)
            {
                Console.WriteLine("Missing arguments. Cannot initialize. Try again.");
                return;
            }

            var testCases = new CsvModel().GetTestCases(parser.Datasource).ToList();


            foreach (var tc in testCases)
            {
                Console.WriteLine("{0} - {1} - {2} - {3}", tc.TestId, tc.Type, tc.Url, tc.ReferenceLocation);
                //Console.WriteLine("{0} - {1} - {3}", tc.DateAdded, tc.DateModified, tc.Release, tc.Description);
            }

            var directory = Directory.CreateDirectory(parser.Directory);
            var reportDirectory = directory.CreateSubdirectory(parser.ReportName);

            var controller = new PreviewController();
            var results = controller.RunTests(testCases, reportDirectory);

            var reportDict = new Dictionary<string, object>();
            reportDict.Add("reportname", parser.ReportName);
            reportDict.Add("result", results);

            var view = new View();
            var html = view.GenerateReport(@"C:\Users\billylee\Documents\visual studio 2013\Projects\Renderly\Renderly\Templates\rendering-results.mustache", reportDict);



            using (var writer = new StreamWriter(Path.Combine(reportDirectory.FullName, "report.html")))
            {
                writer.WriteAsync(html);
            }
            //Console.WriteLine(html);

            //foreach (var r in results)
            //{
            //    Console.WriteLine("Test Case {0} - {1}", r.TestId, r.TestPassed ? "passed" : "failed");
            //}
        }
    }
}
