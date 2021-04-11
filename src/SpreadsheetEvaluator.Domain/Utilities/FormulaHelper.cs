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
            if (ifObjectArray.Count != 3)
            {
                return null;
            }

            var firstIfObject = ifObjectArray[0] as JObject;
            var secondIfObject = ifObjectArray[1] as JObject;
            var thirdIfObject = ifObjectArray[2] as JObject;

            if (firstIfObject == null || secondIfObject == null || thirdIfObject == null)
            {
                return null;
            }

            var formulaOperator = Constants.FormulaOperators.SingleOrDefault(x => firstIfObject.ContainsKey(x.JsonName));

            if (formulaOperator == null)
            {
                return null;
            }

            var elements = firstIfObject[formulaOperator.JsonName] as JArray;

            if (elements == null || secondIfObject["reference"] == null || thirdIfObject["reference"] == null)
            {
                return null;
            }

            var expr = $"IIF({TryParseFormulaReferences(elements, formulaOperator)},{secondIfObject["reference"]},{thirdIfObject["reference"]})";

            return new Formula(expr, formulaOperator);
        }

        public static Formula CreateReferenceFormula(JObject formulaReferenceTypeObject)
        {
            if (formulaReferenceTypeObject.ContainsKey("formula") == false ||
                formulaReferenceTypeObject["formula"]["reference"] == null)
            {
                return null;
            }

            var expr = formulaReferenceTypeObject["formula"]["reference"].ToString();

            return new Formula(expr, Constants.FormulaOperators[10]);
        }
        
        public static Formula CreateNotOperatorFormula(JProperty formulaProperty)
        {
            var expr = Constants.FormulaOperators[5].MathSymbol + " " +
                       JsonObjectHelper.GetValueOfAnyType(formulaProperty);

            return new Formula(expr, Constants.FormulaOperators[5]);
        }

        public static Formula CreateStandardFormula(JProperty formulaProperty)
        {
            var formulaOperator = Constants.FormulaOperators
                .SingleOrDefault(x => formulaProperty.Name.Equals(x.JsonName));

            if (formulaOperator == null)
            {
                return null;
            }

            var expr = TryParseFormulaReferences(formulaProperty.Value as JArray, formulaOperator);

            return new Formula(expr, formulaOperator);
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
                    if (formulaOperator != null && string.IsNullOrEmpty(formulaOperator.MathSymbol) == false)
                    {
                        expr += " " + formulaOperator.MathSymbol + " ";
                    }
                }
            }

            return expr;
        }
    }
}