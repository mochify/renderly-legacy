using System;
using System.IO;

using ManyConsole;
using Renderly.Models.Csv;
using Renderly.Utils;

namespace RenderlyApp.Commands
{
    /// <summary>
    /// This is the ConsoleCommand that handles batch generation of test cases from a file.
    /// The file should be a CSV command with the following column layout:
    /// - test type,
    /// - url/path to retrieve to make your reference image
    /// - release
    /// - description (optional, leave blank if you want)
    /// </summary>
    public class BatchAddTestCaseCommand : ConsoleCommand
    {
        public string InputFile { get; set; }
        public string AppendTestFile { get; set; }
        public string OutputFile { get; set; }

        public BatchAddTestCaseCommand()
        {
            IsCommand("batchadd", "Add test cases as a batch");
            HasRequiredOption("f|file=", "CSV file with the test cases to generate.", s => InputFile = s);
            HasRequiredOption("o|out=", "CSV file to output with test cases.", s => OutputFile = s);
            HasOption("a|append=", "Test Case file to append to", s => AppendTestFile = s);
        }

        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("You are generating tests from {0}", InputFile);
            Console.WriteLine("You are writing out to {0}", OutputFile);

            if (string.IsNullOrWhiteSpace(AppendTestFile))
            {
                Console.WriteLine("Not appending to anything.");
            }
            else
            {
                Console.WriteLine("Appending to {0}.", AppendTestFile);
                File.Copy(AppendTestFile, OutputFile);
            }

            var csvStream = new FileStream(OutputFile, FileMode.Create, FileAccess.ReadWrite);
            var shellFile = new FileStream(InputFile, FileMode.Open, FileAccess.ReadWrite);
            var shellModel = new ShellTestCsvModel(shellFile);
            var fileManager = new RenderlyNativeAssetManager();

            using (var model = new CsvModel(csvStream, fileManager))
            {
                var generator = new TestCaseGenerator(fileManager);
                model.AddTestCases(generator.GenerateTestCases(shellModel.GetTestCases()));
                model.Save();
            }
            
            return 0;
        }
    }
}
