using System.Data;

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

        public static object CalculateMathExpression(this object valueToCompute)
        {
            object returnValue;
            try
            {
                returnValue = new DataTable().Compute(valueToCompute.ToString(), null);
            }
            catch
            {
                returnValue = null;
            }

            return returnValue;
        }
    }
}