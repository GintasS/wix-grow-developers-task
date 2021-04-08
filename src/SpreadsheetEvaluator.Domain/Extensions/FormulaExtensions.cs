using System.Collections.Generic;
using SpreadsheetEvaluator.Domain.Models.MathModels;

namespace SpreadsheetEvaluator.Domain.Extensions
{
    public static class FormulaExtensions
    {
        public static void ReplaceFormulaReferencesWithValues(this Formula formula, List<Cell> cellRow)
        {
            foreach (var cell in cellRow)
            {
                formula.FormulaText = formula.FormulaText.Replace(cell.Key, cell.Value.Value.ToString());
            }
        }
    }
}
