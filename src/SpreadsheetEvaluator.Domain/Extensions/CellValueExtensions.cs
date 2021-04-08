using SpreadsheetEvaluator.Domain.Models.MathModels;

namespace SpreadsheetEvaluator.Domain.Extensions
{
    public static class CellValueExtensions
    {
        public static object GetValue(this CellValue cellValue)
        {
            if (cellValue.Value.IsNumber())
            {
                decimal.TryParse(cellValue.Value.ToString(), out decimal decimalValue);
                return decimalValue;
            }
            else if (cellValue.Value is string)
            {
                return new string(cellValue.Value.ToString());
            }
            else if (cellValue.Value is bool boolValue)
            {
                return boolValue;
            }
            else if (cellValue.Value is Formula formula)
            {
               return new Formula(formula);
            }

            return null;
        }
    }
}
