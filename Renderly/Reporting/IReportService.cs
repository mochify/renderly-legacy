namespace Renderly.Reporting
{
    /// <summary>
    /// This is the interface for report manager/creator objects.
    /// </summary>
    public interface IReportService
    {
        void AddResult(TestResult tr);

        bool GenerateReport();
    }
}
