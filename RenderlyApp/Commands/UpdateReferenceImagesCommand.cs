using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Renderly;
using Renderly.Models.Csv;
using Renderly.Utils;


using ManyConsole;

namespace RenderlyApp.Commands
{
    class UpdateReferenceImagesCommand : ConsoleCommand
    {
        private string ModelFile { get; set; }
        private IEnumerable<int> TestIds { get; set; }

        public UpdateReferenceImagesCommand()
        {
            IsCommand("updateref", "Update reference images for provides test cases.");
            HasRequiredOption("f|file=", "Model file to update test cases in.",
                x => ModelFile = x);
            HasRequiredOption("t|testids=", "Comma-separated list of test IDs to redownload reference images for.",
                x => { TestIds = x.Split(',').Select(Int32.Parse); });
        }

        public override int Run(string[] remainingArguments)
        {
            var fileManager = new RenderlyNativeAssetManager();
            var fstream = new FileStream(ModelFile, FileMode.Open, FileAccess.Read);
            using (var model = new CsvModel(fstream, fileManager))
            {
                var predicate = PredicateBuilder.False<TestCase>();
                foreach (var i in TestIds)
                {
                    predicate = predicate.Or(x => x.TestId == i);
                }

                var updateTests = model.GetTestCases(predicate.Compile());
                foreach (var t in updateTests)
                {
                    fileManager.Fetch(t.SourceLocation, t.ReferenceLocation);
                }
            }

            return 0;
        }
    }
}
