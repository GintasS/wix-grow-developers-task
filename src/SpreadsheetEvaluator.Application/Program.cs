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
            // Fetching services.
            var serviceProvider = Startup.InitServiceProvider();
            var hubService = serviceProvider.GetService<IHubService>();
            var formulaEvaluatorService = serviceProvider.GetService<IFormulaEvaluatorService>();
            var spreadsheetCreationService = serviceProvider.GetService<ISpreadsheetCreationService>();
            var jsonHelper = serviceProvider.GetService<JsonHelper>();

            // Get Raw Json string from Hub API.
            var rawJson = hubService.GetJobs();

            // Parse Json string to object.
            var parsedJsonObject = JsonConvert.DeserializeObject<JobsRawResponse>(rawJson);

            // Parse Json object to jobs.
            var parsedJobs = spreadsheetCreationService.Create(parsedJsonObject);

            // Compute formulas
            var computedJobs = formulaEvaluatorService.ComputeFormulas(parsedJobs);

            // Get object to serialize.
            var serializedResponse = jsonHelper.SerializeJobs(computedJobs);

            // Serialize payload and send to Post API.
            var payload = JsonConvert.SerializeObject(
                serializedResponse,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            hubService.PostJobs(parsedJsonObject.SubmissionUrl, payload);
        }
    }
}
