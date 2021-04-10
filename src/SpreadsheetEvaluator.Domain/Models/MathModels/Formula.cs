using SpreadsheetEvaluator.Domain.Models.Enums;

namespace SpreadsheetEvaluator.Domain.Models.MathModels
{
    public class Formula
    {
        public string Text { get; set; }
        public FormulaOperator FormulaOperator { get; set; }

        public Formula()
        {

        }

        public Formula(Formula formula)
        {
            Text = new string(formula.Text);
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