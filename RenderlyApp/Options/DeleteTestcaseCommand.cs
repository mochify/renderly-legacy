using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManyConsole;
using Renderly.Models;

namespace RenderlyApp.Options
{
    public class DeleteTestcaseCommand : ConsoleCommand
    {
        private string ModelFile { get; set; }
        private string OutputFile { get; set; }
        private IEnumerable<DateTime> Dates { get; set; }
        private IEnumerable<string> Releases { get; set; }
        private IEnumerable<int> TestIds { get; set; }

        public DeleteTestcaseCommand()
        {
            IsCommand("deletetest", "Delete test cases (and their reference images) from a model");
            HasRequiredOption("f|file=", "Model file to delete test cases from.", s => ModelFile = s);
            HasOption("d|dates=", "Comma-separated list of dates to delete test cases for. MM-DD-YYYY format.",
                x => { Dates = x.Split(',').Select(DateTime.Parse); });
            HasOption("r|releases=", "Comma-separated list of releases to delete test cases for",
                x => { Releases = x.Split(','); });
            HasOption("t|testids=", "Comma-separated list of test IDs to delete",
                x => { TestIds = x.Split(',').Select(Int32.Parse); });
            HasOption("o|outfile=", "File to save test cases to. If no file is specified, the input file is overwritten.",
                x => OutputFile = x);
        }


        public override int Run(string[] remainingArguments)
        {
            var model = new CsvModel(ModelFile);

            if (Dates == null && Releases == null && TestIds == null)
            {
                throw new ArgumentException("Please specify at least one of (dates, release, test Ids).");
            }

            var predicate = PredicateBuilder.False<TestCase>();
            if (Dates != null)
            {
                foreach (var d in Dates)
                {
                    predicate = predicate.Or(x => x.DateAdded.Date == d.Date);
                }
            }

            if (Releases != null)
            {
                foreach(var s in Releases)
                {
                    predicate = predicate.Or(x => x.Release == s);
                }
            }

            if (TestIds != null)
            {
                foreach(var t in TestIds)
                {

                    predicate = predicate.Or(x => x.TestId == t);
                }
            }

            var deleted = model.Delete(predicate.Compile());
            Console.WriteLine("Deleted {0} test cases from {1}", deleted, ModelFile);

            
            model.Serialize(OutputFile);

            return 0;
        }
    }
}