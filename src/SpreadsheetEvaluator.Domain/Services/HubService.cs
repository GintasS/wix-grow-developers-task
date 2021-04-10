using Microsoft.Extensions.Options;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Utilities;

namespace SpreadsheetEvaluator.Domain.Services
{
    public class HubService : IHubService
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly HttpClientHelper _httpClientHelper;

        public HubService(IOptionsMonitor<ApplicationSettings> configuration, HttpClientHelper httpClientHelper)
        {
            _applicationSettings = configuration.CurrentValue;
            _httpClientHelper = httpClientHelper;
        }

        public string GetJobs()
        {
            return _httpClientHelper.GetStringContentFromUrl(_applicationSettings.HubApiUrlGetJobs);
        }

        public void PostJobs(string submissionUrl, string payload)
        {
            _httpClientHelper.PostStringContentToUrl(submissionUrl, payload);
        }
    }
}
