using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SpreadsheetEvaluator.Domain;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Utilities;

namespace SpreadsheetEvaluator.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize DI.
            var serviceProvider = Startup.InitServiceProvider();
            
            // Get all the necessary services, helpers.
            var hubService = serviceProvider.GetService<IHubService>();
            var formulaEvaluatorService = serviceProvider.GetService<IFormulaEvaluatorService>();
            var spreadsheetCreationService = serviceProvider.GetService<ISpreadsheetCreationService>();
            var jobsPostRequestHelper = serviceProvider.GetService<JobsPostRequestHelper>();

            // Get Jobs from the Hub Api.
            var json = hubService.GetJobs();

            // Parse Json string to object.
            var jobsRawResponse = JsonConvert.DeserializeObject<JobsRawResponse>(json);

            // Parse Json object to actual jobs list we can use.
            var createdJobs = spreadsheetCreationService.Create(jobsRawResponse);

            // Compute formulas for cells.
            var computedJobs = formulaEvaluatorService.ComputeFormulas(createdJobs);

            // Create a post request to send to the Hub Api.
            var jobsPostRequest = jobsPostRequestHelper.CreatePostRequest(computedJobs);

            // Serialize the payload.
            var payload = JsonConvert.SerializeObject(
                jobsPostRequest,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            // Send the payload to the Hub Api.
            hubService.PostJobs(jobsRawResponse.SubmissionUrl, payload);
        }
    }
}
