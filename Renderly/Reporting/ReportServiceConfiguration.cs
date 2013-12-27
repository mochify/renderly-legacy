namespace Renderly.Reporting
{
    public class ReportServiceConfiguration
    {
        private bool _copyReferenceImages;
        private bool _reportSuccesses;
        private string _outputPath;
        private string _reportName;
        private string _templateDirectory;
        private IReportView _view;
        private string _outputImageType = "jpg";

        public bool CopyReferenceImages
        {
            get { return _copyReferenceImages; }
            set { _copyReferenceImages = value; }
        }

        public bool DisplaySuccesses
        {
            get { return _reportSuccesses; }
            set { _reportSuccesses = value; }
        }

        public string OutputDirectory
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }

        public string ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }

        public IReportView ReportView
        {
            get { return _view; }
            set { _view = value; }
        }

        public string TemplateDirectory
        {
            get { return _templateDirectory; }
            set { _templateDirectory = value; }
        }

        public string OutputImageType
        {
            get { return _outputImageType; }
            set { _outputImageType = value; }
        }
    }
}
