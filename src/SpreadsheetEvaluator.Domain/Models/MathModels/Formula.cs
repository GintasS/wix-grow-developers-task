using SpreadsheetEvaluator.Domain.Models.Enums;

namespace SpreadsheetEvaluator.Domain.Models.MathModels
{
    public class Formula
    {
        public string FormulaText { get; set; }
        public FormulaOperator FormulaOperator { get; set; }

        public Formula()
        {

        }

        public Formula(Formula formula)
        {
            FormulaText = new string(formula.FormulaText);
            FormulaOperator = formula.FormulaOperator;
        }
    }

    public class FormulaOperator
    {
        public string JsonName { get; set; }
        public string MathSymbol { get; set; }
        public FormulaResultType FormulaResultType { get; set; }
    }
}