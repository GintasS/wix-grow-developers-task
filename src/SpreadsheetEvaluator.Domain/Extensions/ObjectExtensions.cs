using SpreadsheetEvaluator.Domain.Models.Enums;
using SpreadsheetEvaluator.Domain.Models.MathModels;

namespace SpreadsheetEvaluator.Domain.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public static CellType? TryGetCellTypeFromValue(this object value)
        {
            if (value.IsNumber())
            {
                return CellType.Number;
            }
            else if (value is string)
            {
                return CellType.Text;
            }
            else if (value is bool)
            {
                return CellType.Boolean;
            }
            else if (value is Formula)
            {
                return CellType.Formula;
            }
            else if (value is CellValue cellValue)
            {
                return cellValue.CellType;
            }

            return null;
        }
    }
}