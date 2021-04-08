using SpreadsheetEvaluator.Domain.MathModels;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Extensions
{
    public static class FormulaExtensions
    {
        public static void ReplaceFormulaReferencesWithValues(this Formula formula, List<Cell> cellRow)
        {
            foreach (var replacingReference in cellRow)
            {
                formula.FormulaText = formula.FormulaText.Replace(replacingReference.Key, replacingReference.Value.Value.ToString());
            }
        }
    }
}
