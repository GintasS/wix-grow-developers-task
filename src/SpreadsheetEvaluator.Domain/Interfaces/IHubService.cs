using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetEvaluator.Domain.Interfaces
{
    public interface IHubService
    {
        string GetJobs();
        void PostJobs(string submissionUrl, string payload);
    }
}
