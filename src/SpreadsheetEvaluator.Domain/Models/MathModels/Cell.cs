using SpreadsheetEvaluator.Domain.Configuration;
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
        public bool IsCellUndefined => CellType == CellType.Undefined;

        public CellValue(object value)
        {
            UpdateCell((dynamic)value);
        }

        public void UpdateCell<T>(T value) where T : struct
        {
            if (value.IsNumber())
            {
                decimal.TryParse(value.ToString(), out decimal decimalResult);
                Value = decimalResult;
                CellType = CellType.Number;
            }
        }

        public void UpdateCell(string value)
        {
            Value = new string(value);
            CellType = CellType.Text;
        }

        public void UpdateCell(bool value)
        {
            Value = value;
            CellType = CellType.Boolean;
        }

        public void UpdateCell(Formula value)
        {
            Value = new Formula(value);
            CellType = CellType.Formula;
        }

        public void UpdateCell(CellValue cellValue)
        {
            Value = cellValue.Value;
            CellType = cellValue.CellType;
        }

        public void SetCellAsErrorCell()
        {
            CellType = CellType.Error;
            Value = Constants.Error.MismatchingTypes;
        }
    }
}
