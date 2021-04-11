using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SpreadsheetEvaluator.Application.Enums;
using SpreadsheetEvaluator.Domain;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Utilities;

namespace SpreadsheetEvaluator.Application
{
    internal class Program
    {
        private static int Main()
        {
            // 1. Initialize DI.
            var serviceProvider = Startup.InitServiceProvider();
            
            // 2. Get all the necessary services, helpers.
            var hubService = serviceProvider.GetService<IHubService>();
            var formulaEvaluatorService = serviceProvider.GetService<IFormulaEvaluatorService>();
            var spreadsheetCreationService = serviceProvider.GetService<ISpreadsheetCreationService>();
            var jobsPostRequestHelper = serviceProvider.GetService<JobsPostRequestHelper>();

            // 3. Get Jobs from the Hub Api.
            JobsGetRawResponse jobsGetRawResponse = null;

            try
            {
                var getJobHttpResponse = hubService.GetJobs();
                var jsonString = getJobHttpResponse.Content.ReadAsStringAsync().Result;
                jobsGetRawResponse = JsonConvert.DeserializeObject<JobsGetRawResponse>(jsonString);
            }
            catch(Exception ex)
            {
                Console.WriteLine(Constants.ExitMessages.FailedToGetJobs, ex.Message);

                return (int)ExitCode.ErrorGetJobs;
            }

            // 4. Parse Json object to actual jobs list that we can use.
            var createdJobs = spreadsheetCreationService.Create(jobsGetRawResponse);

            // 5. Compute formulas for cells.
            var computedJobs = formulaEvaluatorService.ComputeFormulas(createdJobs);

            // 6. Create a post request to send to the Hub Api.
            var jobsPostRequest = jobsPostRequestHelper.CreatePostRequest(computedJobs);

            // 7. Serialize the payload.
            var payload = JsonConvert.SerializeObject(
                jobsPostRequest,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            // 8. Send the payload to the Hub Api.
            JobsPostResponse jobsPostResponse = null;

            try
            {
                var postJobsHttpResponse = hubService.PostJobs(jobsGetRawResponse.SubmissionUrl, payload);
                var responseText = postJobsHttpResponse.Content.ReadAsStringAsync().Result;
                jobsPostResponse = JsonConvert.DeserializeObject<JobsPostResponse>(responseText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.ExitMessages.FailedToPostJobs, ex.Message, jobsPostResponse?.Error);

                return (int)ExitCode.ErrorPostJobs;
            }

            // 9. Display a success message if our results were received and they were correct.
            Console.WriteLine(jobsPostResponse.Message);

            return (int) ExitCode.Success;
        }
    }
}
