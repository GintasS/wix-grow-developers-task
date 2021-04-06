﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Models.Responses
{
    public class JobsPostModel
    {
        [JsonProperty("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonProperty("data")]
        [JsonRequired]
        public List<List<JobsPostValueModel>> Values { get; set; }
    }

    public class JobsPostValueModel
    {
        [JsonProperty("value")]
        public Values Values { get; set; }

        [JsonProperty("error")]
        public string error { get; set; }
    }

    public class Values
    {
        public bool? boolean { get; set; }
        public decimal? number { get; set; }
        public string text { get; set; }
    }
}