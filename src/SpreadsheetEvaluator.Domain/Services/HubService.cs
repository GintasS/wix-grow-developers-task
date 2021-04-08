using Microsoft.Extensions.Options;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Utilities;

namespace SpreadsheetEvaluator.Domain.Services
{
    // TODO: We may not catch constant values (now we are picking references nicely).
    // TODO: Make this parser more robust
    // TODO: Update/Remove Nuget packages
    // TODO: Clean up code using suggestions from VS
    // TODO: Move classes to folders
    // TODO: Better naming for everything almost
    // TODO: Tests
    // TODO: Move code out of this and other classes
    // TODO: Find edge cases to break your code
    // TODO: Remove unused files
    // TODO: Move hard-coded values to constants
    // TODO: add gitignore to prevent .exe and dlls to be included into the project
    // TODO: add necessary comments, especially in the recursive part.
    // TODO: Validation everywhere
    // TODO: Remove unused includes
    // TODO: Add something to changelog.
    // TODO: Exception handling where needed
    // TODO: If formula comes first, things can break and we will not save value.

    public partial class HubService : IHubService
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
            //var content = File.ReadAllText(@"C:\Users\Gintautas\Documents\Visual Studio 2019\Projects\Wix-grow-developers-task\src\SpreadsheetEvaluator.Domain\test.json");
        }

        public void PostJobs(string submissionUrl, string payload)
        {
            _httpClientHelper.PostStringContentToUrl(submissionUrl, payload);
        }
    }
}
