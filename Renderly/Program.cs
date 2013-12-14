using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CsvHelper;
using Nustache.Core;

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

            //var csv = new CsvReader(new StreamReader(parser.Datasource));
            //csv.Configuration.HasHeaderRecord = true;
            //while (csv.Read())
            //{
            //    Console.WriteLine("{0}", csv.GetField<int>(0));
            //    Console.WriteLine("{0}", csv.GetField<string>(1));
            //}

            var testCases = new CsvModel().GetTestCases(parser.Datasource).ToList();


            foreach (var tc in testCases)
            {
                Console.WriteLine("{0} - {1} - {2} - {3}", tc.TestId, tc.Type, tc.Url, tc.ReferenceLocation);
                Console.WriteLine("{0} - {1} - {3}", tc.DateAdded, tc.DateModified, tc.Release, tc.Description);
            }

            var controller = new PreviewController();
            controller.RunTests(testCases);
        }
    }
}
