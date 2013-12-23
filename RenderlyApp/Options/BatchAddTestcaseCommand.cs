using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ManyConsole;
using Renderly.Models;
using Renderly.Utils;

namespace RenderlyApp.Options
{
    /// <summary>
    /// This is the ConsoleCommand that handles batch generation of test cases from a file.
    /// The file should be a CSV command with the following column layout:
    /// - test type,
    /// - url/path to retrieve to make your reference image
    /// - release
    /// - description (optional, leave blank if you want)
    /// </summary>
    public class BatchAddTestcaseCommand : ConsoleCommand
    {
        public string InputFile { get; set; }
        public string AppendTestFile { get; set; }
        public string OutputFile { get; set; }
        public bool ModifyInPlace { get; set; }

        public BatchAddTestcaseCommand()
        {
            IsCommand("batchadd", "Add test cases as a batch");
            HasRequiredOption("f|file=", "CSV file with the test cases to generate.", s => InputFile = s);
            HasRequiredOption("o|out=", "CSV file to output with test cases.", s => OutputFile = s);
            HasOption("a|append=", "Test Cases to append to", s => AppendTestFile = s);
        }


        public override int Run(string[] remainingArguments)
        {
            var inputCsvFile = OutputFile;
            if (string.IsNullOrWhiteSpace(AppendTestFile))
            {
                Console.WriteLine("Not appending to anything.");
            }
            else
            {
                Console.WriteLine("Appending to {0}", AppendTestFile);
                inputCsvFile = AppendTestFile;
            }

            Console.WriteLine("You are generating tests from {0}", InputFile);
            Console.WriteLine("You are writing out to {0}", OutputFile);

            var model = new CsvModel(inputCsvFile);
            var shellModel = new ShellTestCsvModel(InputFile);
            model.GenerateTestCases(shellModel.GetTestCases());
            model.Serialize(OutputFile);

            return 0;
        }
    }
}
