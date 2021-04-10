using Microsoft.Extensions.Options;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Models.Requests;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public class JobsPostRequestHelper
    {
        private readonly ApplicationSettings _applicationSettings;

        public JobsPostRequestHelper(IOptionsMonitor<ApplicationSettings> configuration)
        {
            _applicationSettings = configuration.CurrentValue;
        }

        public JobsPostRequest CreatePostRequest(List<JobComputed> jobsComputedList)
        {
            var jobsPostRequest = new JobsPostRequest
            {
                Email = _applicationSettings.DeveloperEmailAddress,
                Jobs = new List<JobsPostModel>()
            };

            var jobs = jobsComputedList.Select(x => new {
                Id = x.Id,
                Cells = x.Cells
            }).ToList();

            for (var jobIndex = 0; jobIndex < jobs.Count; jobIndex++)
            {
                var jobsPostModel = new JobsPostModel
                {
                    Id = jobs[jobIndex].Id,
                    Values = new List<List<JobsPostValueModel>>()
                };

                for (var cellRowIndex = 0; cellRowIndex < jobs[jobIndex].Cells.Count; cellRowIndex++)
                {
                    if (cellRowIndex >= jobsPostModel.Values.Count)
                    {
                        jobsPostModel.Values.Add(new List<JobsPostValueModel>());
                    }

                    for (var cellIndex = 0; cellIndex < jobs[jobIndex].Cells[cellRowIndex].Count(); cellIndex++)
                    {
                        var individualCell = jobs[jobIndex].Cells[cellRowIndex][cellIndex];
                        jobsPostModel.Values[cellRowIndex].Add(new JobsPostValueModel(individualCell.Value));
                    }
                }

                jobsPostRequest.Jobs.Add(jobsPostModel);
            }

            return jobsPostRequest;
        }
    }
}
