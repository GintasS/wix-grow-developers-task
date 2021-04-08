using Microsoft.Extensions.Options;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Models.Enums;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Models.Requests;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public class JsonHelper
    {
        private readonly ApplicationSettings _applicationSettings;

        public JsonHelper(IOptionsMonitor<ApplicationSettings> configuration)
        {
            _applicationSettings = configuration.CurrentValue;
        }

        public JobsPostRequest SerializeJobs(List<ComputedJob> jobsList)
        {
            var jobsPostRequest = new JobsPostRequest
            {
                Email = _applicationSettings.DeveloperEmailAddress,
                Jobs = new List<JobsPostModel>()
            };

            var jobs = jobsList.Select(x => new {
                Id = x.Id,
                Cells = x.Cells
            }).ToList();

            for (var jobIndex = 0; jobIndex < jobs.Count; jobIndex++)
            {
                var jobsPostModel = new JobsPostModel()
                {
                    Id = jobs[jobIndex].Id,
                    Values = new List<List<JobsPostValueModel>>() { }
                };

                for (var cellRowIndex = 0; cellRowIndex < jobs[jobIndex].Cells.Count; cellRowIndex++)
                {
                    var cellRow = jobs[jobIndex].Cells[cellRowIndex];

                    if (cellRowIndex >= jobsPostModel.Values.Count)
                    {
                        jobsPostModel.Values.Add(new List<JobsPostValueModel>());
                    }

                    for (var cellIndex = 0; cellIndex < jobs[jobIndex].Cells[cellRowIndex].Count(); cellIndex++)
                    {
                        var individualCell = jobs[jobIndex].Cells[cellRowIndex][cellIndex];

                        var jobsPostValueModel = new JobsPostValueModel()
                        {
                            Values = new Values()
                        };

                        if (individualCell.Value.CellType == CellType.Number)
                        {
                            jobsPostValueModel.Values.number = (decimal)individualCell.Value.Value;
                        }
                        else if (individualCell.Value.CellType == CellType.Text)
                        {
                            jobsPostValueModel.Values.text = individualCell.Value.Value.ToString();
                        }
                        else if (individualCell.Value.CellType == CellType.Bool)
                        {
                            jobsPostValueModel.Values.boolean = (bool)individualCell.Value.Value;
                        }
                        else if (individualCell.Value.IsErrorCell)
                        {
                            jobsPostValueModel.error = individualCell.Value.Value.ToString();
                            jobsPostValueModel.Values = null;
                        }

                        jobsPostModel.Values[cellRowIndex].Add(jobsPostValueModel);
                    }
                }

                jobsPostRequest.Jobs.Add(jobsPostModel);
            }

            return jobsPostRequest;
        }
    }
}
