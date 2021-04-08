using System.Data;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public static class CalculationHelper
    {
        public static object ComputeResult(object value)
        {
            object computationResult = null;
            try
            {
                computationResult = new DataTable().Compute(value.ToString(), null);
            }
            catch (EvaluateException ex)
            {
                throw ex;
            }

            return computationResult;
        }
    }
}