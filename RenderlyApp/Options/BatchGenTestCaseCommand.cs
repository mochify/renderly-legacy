using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManyConsole;

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
    public class BatchGenTestCaseCommand : ConsoleCommand
    {
        public string InputFile;
        public string AppendTestFile;

        public BatchGenTestCaseCommand()
        {
            IsCommand("batchgen");
            HasRequiredOption("f|file=", "CSV file with the test cases to generate.", s => InputFile = s);
            HasOption("a|append=", "Test Cases to append to", s => AppendTestFile = s);
        }


        public override int Run(string[] remainingArguments)
        {
            if (string.IsNullOrWhiteSpace(AppendTestFile))
            {
                Console.WriteLine("Not appending to anything.");
            }
            else
            {
                Console.WriteLine("Appending to {0}", AppendTestFile);
            }

            Console.WriteLine("You are generating tests from {0}", InputFile);

            return 0;
        }
    }
}
