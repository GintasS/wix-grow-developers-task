using System.Linq;
using Newtonsoft.Json.Linq;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Models.MathModels;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public static class FormulaHelper
    {
        public static Formula CreateIfFormula(JArray ifObjectArray)
        {
            var firstIfObject = ifObjectArray[0] as JObject;
            var secondIfObject = ifObjectArray[1] as JObject;
            var thirdIfObject = ifObjectArray[2] as JObject;

            if (firstIfObject == null || secondIfObject == null || thirdIfObject == null)
            {
                return null;
            }

            var formulaOperator = Constants.FormulaOperators.Single(x => firstIfObject.ContainsKey(x.JsonName));
            var elements = firstIfObject[formulaOperator.JsonName] as JArray;

            if (elements == null || secondIfObject["reference"] == null || thirdIfObject["reference"] == null)
            {
                return null;
            }

            var expr = $"IIF({TryParseFormulaReferences(elements, formulaOperator)},{secondIfObject["reference"]},{thirdIfObject["reference"]})";

            return new Formula
            {
                Text = expr,
                FormulaOperator = formulaOperator
            };
        }

        public static Formula CreateReferenceFormula(JObject formulaReferenceTypeObject)
        {
            if (formulaReferenceTypeObject.ContainsKey("formula") == false ||
                formulaReferenceTypeObject["formula"]["reference"] == null)
            {
                return null;
            }

            return new Formula
            {
                Text = formulaReferenceTypeObject["formula"]["reference"].ToString(),
                FormulaOperator = Constants.FormulaOperators[10]
            };
        }

        public static Formula CreateLogicalOperatorsFormula(JProperty formulaProperty)
        {
            var formulaOperator = Constants.FormulaOperators
                .SingleOrDefault(x => formulaProperty.Name.Equals(x.JsonName));

            return new Formula
            {
                Text = TryParseFormulaReferences(formulaProperty.Value as JArray, formulaOperator),
                FormulaOperator = formulaOperator
            };
        }

        public static Formula CreateNotOperatorFormula(JProperty formulaProperty)
        {
            return new Formula
            {
                Text = Constants.FormulaOperators[5].MathSymbol + " " + JsonObjectHelper.GetValueOfAnyType(formulaProperty),
                FormulaOperator = Constants.FormulaOperators[5]
            };
        }

        public static Formula CreateStandardFormula(JProperty formulaProperty)
        {
            var formulaOperator = Constants.FormulaOperators
                .SingleOrDefault(x => formulaProperty.Name.Equals(x.JsonName));

            return new Formula
            {
                Text = TryParseFormulaReferences(formulaProperty.Value as JArray, formulaOperator),
                FormulaOperator = formulaOperator
            };
        }

        private static string TryParseFormulaReferences(JArray elements, FormulaOperator formulaOperator)
        {
            var expr = string.Empty;
            for (var i = 0; i < elements.Count; i++)
            {
                if (elements[i]["reference"] != null)
                {
                    expr += elements[i]["reference"];
                }
                else if (elements[i]["value"] != null)
                {
                    var jsonObject = elements[i]["value"] as JObject;
                    expr += JsonObjectHelper.GetValueOfAnyType(jsonObject);
                }

                if (i + 1 < elements.Count)
                {
                    if (string.IsNullOrEmpty(formulaOperator.MathSymbol) == false)
                    {
                        expr += " " + formulaOperator.MathSymbol + " ";
                    }
                }
            }

            return expr;
        }
    }
}