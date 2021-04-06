using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetEvaluator.Domain.Models.Responses
{
    public class JobsPostRequest
    {
        [JsonProperty("email")]
        [JsonRequired]
        public string Email { get; set; }

        [JsonProperty("results")]
        [JsonRequired]
        public List<JobsPostModel> Jobs { get; set; }
    }
}
