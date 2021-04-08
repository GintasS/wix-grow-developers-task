namespace SpreadsheetEvaluator.Domain.Interfaces
{
    public interface IHubService
    {
        string GetJobs();
        void PostJobs(string submissionUrl, string payload);
    }
}
