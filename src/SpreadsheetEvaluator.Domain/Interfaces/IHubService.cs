namespace SpreadsheetEvaluator.Domain.Interfaces
{
    public interface IHubService
    {
        public string GetJobs();
        public void PostJobs(string submissionUrl, string payload);
    }
}
