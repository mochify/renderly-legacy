using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NDesk.Options;

namespace RenderlyApp
{
    class OptionParser
    {
        private IList<int> _testCasesToRun = new List<int>();
        private IList<string> _testClassesToRun = new List<string>();
        private bool _showHelp = false;
        private string _reportName = "";
        private string _dataSource = "";
        private string _directory = "";
        private string _templateDir = "";

        public string Datasource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        public IList<int> TestCasesToRun
        {
            get { return _testCasesToRun; }
        }

        public IList<string> TestClassesToRun
        {
            get { return _testClassesToRun; }
        }

        public bool ShowHelp
        {
            get { return _showHelp; }
            set { _showHelp = value; }
        }

        public string Directory
        {
            get { return _directory; }
            set { _directory = value; }
        }

        public string ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }

        public string TemplateDir
        {
            get { return _templateDir; }
            set { _templateDir = value; }
        }

        private OptionSet GetParser()
        {
            var os = new OptionSet
            {
                {
                    "d|datasource=",
                    "The datasource to use to get test cases from",
                    (string x) => Datasource = x
                },
                {
                    "i|id=",
                    "The test case number to use from the datasource (optional). Can be specified more than once.",
                    (int x) => TestCasesToRun.Add(x)
                },
                {
                    "c|class=",
                    "The class of tests to run from the datasource. Will run all the tests with a specific class. Can be specified more than once",
                    (string x) => TestClassesToRun.Add(x)
                },
                {
                    "n|name=",
                    "The name of the report to generate",
                    (string x) => ReportName = x
                },
                {
                    "o|dir=",
                    "The parent directory to write the report to. This will be combined with the Report name argument",
                    (string x) => Directory = x
                },
                {
                    "t|templatedir=",
                    "The directory where report templates are stored",
                    (string x) => TemplateDir = x
                },
                {
                    "h|help",
                    "Show this help message and exit",
                    x => ShowHelp = x != null
                }
            };

            return os;
        }

        public void ParseArgs(IEnumerable<string> args)
        {
            GetParser().Parse(args);
        }

        public void ShowHelpMessage()
        {
            Console.WriteLine("\nHow to run: thisprogram.exe [options]");
            GetParser().WriteOptionDescriptions(Console.Out);
        }
    }
}
