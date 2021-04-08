using SpreadsheetEvaluator.Domain.MathModels;
using SpreadsheetEvaluator.Domain.Models.Enums;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Configuration
{
    public class Constants
    {
        public static List<FormulaOperator> FormulaOperators = new List<FormulaOperator>() 
        {
            new FormulaOperator
            {
                JsonName = "sum",
                MathSymbol = "+",
                FormulaOperatorType = FormulaOperatorType.sum,
                FormulaResultType = ResultType.Number
            },
            new FormulaOperator
            {
                JsonName = "multiply",
                MathSymbol = "*",
                FormulaOperatorType = FormulaOperatorType.multiply,
                FormulaResultType = ResultType.Number
            },
            new FormulaOperator
            {
                JsonName = "divide",
                MathSymbol = "/",
                FormulaOperatorType = FormulaOperatorType.divide,
                FormulaResultType = ResultType.Number
            },
            new FormulaOperator
            {
                JsonName = "is_greater",
                MathSymbol = ">",
                FormulaOperatorType = FormulaOperatorType.is_greater,
                FormulaResultType = ResultType.Bool
            },
            new FormulaOperator
            {
                JsonName = "is_equal",
                MathSymbol = "=",
                FormulaOperatorType = FormulaOperatorType.is_equal,
                FormulaResultType = ResultType.Bool
            },
            new FormulaOperator
            {
                JsonName = "not",  // not covered in if statement
                MathSymbol = "NOT",
                FormulaOperatorType = FormulaOperatorType.not,
                FormulaResultType = ResultType.Bool
            },
            new FormulaOperator
            {
                JsonName = "and",
                MathSymbol = " AND ",
                FormulaOperatorType = FormulaOperatorType.and,
                FormulaResultType = ResultType.Bool
            },
            new FormulaOperator
            {
                JsonName = "or",
                MathSymbol = "OR",
                FormulaOperatorType = FormulaOperatorType.or,
                FormulaResultType = ResultType.Bool
            },
            new FormulaOperator
            {
                JsonName = "if",
                MathSymbol = "IIF",
                FormulaOperatorType = FormulaOperatorType.iff,
                FormulaResultType = ResultType.Undefined
            },
            new FormulaOperator
            {
                JsonName = "concat",
                MathSymbol = null,
                FormulaOperatorType = FormulaOperatorType.concat,
                FormulaResultType = ResultType.Text
            },
            new FormulaOperator
            {
                JsonName = "reference",
                MathSymbol = null,
                FormulaOperatorType = FormulaOperatorType.undefined,
                FormulaResultType = ResultType.Reference
            }
        };
    }
}
