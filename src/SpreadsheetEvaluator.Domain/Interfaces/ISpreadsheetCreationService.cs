﻿using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Models.Responses;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Interfaces
{
    public interface ISpreadsheetCreationService
    {
        List<SingleJob> Create(JobsRawResponse jobsRawResponse);
    }
}
