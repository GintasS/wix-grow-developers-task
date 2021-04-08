using SpreadsheetEvaluator.Domain.Models.MathModels;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Interfaces
{
    public interface IFormulaEvaluatorService
    {
        public List<ComputedJob> ComputeFormulas(List<SingleJob> jobsList);
    }
}