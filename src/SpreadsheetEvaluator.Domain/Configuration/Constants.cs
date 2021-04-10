using SpreadsheetEvaluator.Domain.Models.Enums;
using System.Collections.Generic;
using SpreadsheetEvaluator.Domain.Models.MathModels;

namespace SpreadsheetEvaluator.Domain.Configuration
{
    public class Constants
    {
        public static readonly List<FormulaOperator> FormulaOperators = new List<FormulaOperator> 
        {
            new FormulaOperator
            {
                JsonName = "sum",
                MathSymbol = "+",
                FormulaResultType = FormulaResultType.Number
            },
            new FormulaOperator
            {
                JsonName = "multiply",
                MathSymbol = "*",
                FormulaResultType = FormulaResultType.Number
            },
            new FormulaOperator
            {
                JsonName = "divide",
                MathSymbol = "/",
                FormulaResultType = FormulaResultType.Number
            },
            new FormulaOperator
            {
                JsonName = "is_greater",
                MathSymbol = ">",
                FormulaResultType = FormulaResultType.Boolean
            },
            new FormulaOperator
            {
                JsonName = "is_equal",
                MathSymbol = "=",
                FormulaResultType = FormulaResultType.Boolean
            },
            new FormulaOperator
            {
                JsonName = "not",  // not covered in if statement
                MathSymbol = "NOT",
                FormulaResultType = FormulaResultType.Boolean
            },
            new FormulaOperator
            {
                JsonName = "and",
                MathSymbol = " AND ",
                FormulaResultType = FormulaResultType.Boolean
            },
            new FormulaOperator
            {
                JsonName = "or",
                MathSymbol = "OR",
                FormulaResultType = FormulaResultType.Boolean
            },
            new FormulaOperator
            {
                JsonName = "if",
                MathSymbol = "IIF",
                FormulaResultType = FormulaResultType.Undefined
            },
            new FormulaOperator
            {
                JsonName = "concat",
                MathSymbol = null,
                FormulaResultType = FormulaResultType.Text
            },
            new FormulaOperator
            {
                JsonName = "reference",
                MathSymbol = null,
                FormulaResultType = FormulaResultType.Reference
            }
        };

        public static readonly List<string> StandardOperatorNames = new List<string>()
        {
            "sum",
            "multiply",
            "divide"
        };

        public static readonly List<string> LogicalOperatorNames = new List<string>()
        {
            "is_greater",
            "is_equal",
            "and",
            "or",
            "not"
        };

        public struct Formula
        {
            public const string Concat = "concat";
            public const string Not = "not";
        }

        public struct JobJson
        {
            public const string Id = "id";
            public const string Data = "data";
            public const string Formula = "formula";
            public const string Value = "value";
            public const string IfValue = "if";
            public const string Reference = "reference";
        }

        public struct Error
        {
            public const string MismatchingTypes = "type does not match";
        }

        public struct HubApi
        {
            public const string PostMediaType = "application/json";
        }
    }
}