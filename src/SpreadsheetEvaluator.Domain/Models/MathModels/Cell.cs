using SpreadsheetEvaluator.Domain.Extensions;
using SpreadsheetEvaluator.Domain.Models.Enums;

namespace SpreadsheetEvaluator.Domain.Models.MathModels
{
    public class Cell
    {
        public string Key { get; set; }
        public CellValue Value { get; set; }
    }

    public class CellValue
    {
        public object Value { get; private set; }
        public CellType CellType { get; private set; } = CellType.Undefined;
        public bool IsFormulaCell => Value is Formula;
        public bool IsErrorCell => CellType == CellType.Error;

        public CellValue()
        {

        }

        public CellValue(object value)
        {
            TryUpdateCell((dynamic)value);
        }

        public void TryUpdateCell(object value)
        {
            var cellType = value.TryGetCellTypeFromValue();

            if (cellType == null)
            {
                return;
            }

            if (value.IsNumber())
            {
                decimal.TryParse(value.ToString(), out decimal decimalValue);
                Value = decimalValue;
            }
            else if (value is string)
            {
                Value = new string(value.ToString());
            }
            else if (value is bool boolValue)
            {
                Value = boolValue;
            }
            else if (value is Formula formula)
            {
                Value = new Formula(formula);
            }
            else if (value is CellValue cellValue)
            {
                Value = cellValue.GetValue();
            }

            CellType = cellType.Value;
        }

        public void SetCellAsErrorCell()
        {
            CellType = CellType.Error;
            Value = "type does not match";
        }
    }
}
