using Newtonsoft.Json.Linq;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetEvaluator.Domain.Services
{
    // TODO: We may not catch constant values (now we are picking references nicely).
    // TODO: Make this parser more robust
    // TODO: Clean up code using suggestions from VS
    // TODO: Better naming for everything almost
    // TODO: Tests
    // TODO: Move code out of this and other classes
    // TODO: Find edge cases to break your code
    // TODO: Remove unused files
    // TODO: Move hard-coded values to constants
    // TODO: add gitignore to prevent .exe and dlls to be included into the project
    // TODO: add necessary comments, especially in the recursive part.
    // TODO: Validation everywhere
    // TODO: Remove unused includes
    // TODO: Add something to changelog.
    // TODO: Exception handling where needed
    // TODO: If formula comes first, things can break and we will not save value.

    public class SpreadsheetCreationService : ISpreadsheetCreationService
    {
        public List<SingleJob> Create(JobsRawResponse jobsRawResponse)
        {
            var createdJobs = new List<SingleJob>();
            foreach (var value in jobsRawResponse.Jobs.Values.Children())
            {
                var ret = RecursiveIteration(value, new SingleJob());
                createdJobs.Add(ret);
            }

            return createdJobs;
        }

        private SingleJob RecursiveIteration(IEnumerable<JToken> tokens, SingleJob singleJob)
        {
            foreach (var token in tokens)
            {
                Console.WriteLine(" CHILDREN: " + token.ToString());

                // We are at the 1 below root, aka, on on the individual job
                if (token is JProperty jProperty && jProperty.Name == "id")
                {
                    singleJob.Id = jProperty.Value.ToString();
                }
                else if (token?.Parent != null && token?.Parent is JProperty jProperty1 && jProperty1.Name == "data" && singleJob.IteratingOverDataArray == false)
                {
                    singleJob.IteratingOverDataArray = true;
                }

                if (singleJob.IteratingOverDataArray)
                {
                    if (token is JArray && token?.Parent is JArray && token.Parent.Count > 1)
                    {
                        if (singleJob.IsMultiArray == false && token.Parent.Count > 1)
                        {
                            singleJob.IsMultiArray = true;
                        }
                        else if (singleJob.IsMultiArray)
                        {
                            singleJob.ResetLetterIndex();
                            singleJob.IncrementCellIndex();
                        }

                        singleJob.IteratingOverInsideArray = true;
                    }

                    if (token is JObject formulaObject && formulaObject.ContainsKey("formula") && singleJob.IsInFormulaObject == false)
                    {
                        // Check for operator existence here.

                        singleJob.IsInFormulaObject = true;
                    }

                    if (singleJob.Id == "job-18")
                    {
                        Console.WriteLine("A");
                    }





                    if (token is JProperty valueObject333 && token?.Parent is JObject jjj && jjj.ContainsKey("value"))
                    {
                        singleJob.FoundValue = true;
                    }
                    else if (token is JProperty valueObject && singleJob.FoundValue)
                    {
                        var t = valueObject.AncestorsAndSelf()
                                               .OfType<JObject>()
                                               .Any(n => n.ContainsKey("formula"));

                        if (t == false)
                        {
                            Cell cell = new Cell
                            {
                                Value = null
                            };
 
                            if (valueObject.Name == "text")
                            {
                                cell.Value = new CellValue(valueObject.Value.ToString());
                            }
                            else if (valueObject.Name == "number")
                            {
                                Decimal.TryParse(valueObject.Value.ToString(), out decimal decimalValue);
                                cell.Value = new CellValue(decimalValue);
                            }
                            else if (valueObject.Name == "boolean")
                            {
                                bool.TryParse(valueObject.Value.ToString(), out bool booleanValue);
                                cell.Value = new CellValue(booleanValue);
                            }

                            singleJob.SetReferenceToValue(cell);
                            singleJob.FoundValue = false;
                            singleJob.HaveEverSeenValue = true;
                        }
                    }
                    else if (token is JProperty formulaPropety && singleJob.IsInFormulaObject)
                    {
                        if (formulaPropety.Name == "if" && singleJob.FoundIfFormula == false)
                        {
                            singleJob.FoundIfFormula = true;
                        }



                        // Finish here.

                        // is_greater, is_equal, not, and, or

                        // Formulas

                    }


                    if (singleJob.IsInFormulaObject)
                    {
                        if (token is JArray ifObject && ifObject.Count == 3 && singleJob.FoundIfFormula)
                        {
                            var firstIfObject = ifObject[0] as JObject;
                            var secondIfObject = ifObject[1] as JObject;
                            var thirdIfObject = ifObject[2] as JObject;

                            var formulaOperator = Constants.FormulaOperators.Single(x => firstIfObject.ContainsKey(x.JsonName));

                            var elements = firstIfObject[formulaOperator.JsonName] as JArray;
                            var expr = "IIF(" + ParseFormulaReferences(elements, formulaOperator) + "," + secondIfObject["reference"] + "," + thirdIfObject["reference"] + ")";

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty sumFormulaObject && sumFormulaObject.Name == "sum")
                        {
                            var sumArray = sumFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => sumFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty multiplyFormulaObject && multiplyFormulaObject.Name == "multiply")
                        {
                            var sumArray = multiplyFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => multiplyFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty divideFormulaObject && divideFormulaObject.Name == "divide")
                        {
                            var sumArray = divideFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => divideFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty isGreaterFormulaObject && isGreaterFormulaObject.Name == "is_greater" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = isGreaterFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => isGreaterFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty isEqualFormulaObject && isEqualFormulaObject.Name == "is_equal" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = isEqualFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => isEqualFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty notFormulaObject && notFormulaObject.Name == "not" && singleJob.FoundIfFormula == false)
                        {
                            var formulaOperator = Constants.FormulaOperators.Single(x => notFormulaObject.Name.Equals(x.JsonName));
                            singleJob.SetFormula(new Formula { FormulaText = "NOT " + JsonObjectHelper.GetValueOfAnyType(notFormulaObject), FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty andFormulaObject && andFormulaObject.Name == "and" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = andFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => andFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty orFormulaObject && orFormulaObject.Name == "or" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = orFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => orFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty concatFormulaObject && concatFormulaObject.Name == "concat" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = concatFormulaObject.Value as JArray;
                            var formulaOperator = Constants.FormulaOperators.Single(x => concatFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.SetFormula(new Formula { FormulaText = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JObject emptyFormulaFieldOnlyReferenceObject && emptyFormulaFieldOnlyReferenceObject.ContainsKey("formula") && emptyFormulaFieldOnlyReferenceObject.ContainsKey("formula") && emptyFormulaFieldOnlyReferenceObject["formula"]["reference"] != null)
                        {
                            singleJob.SetFormula(new Formula { FormulaText = emptyFormulaFieldOnlyReferenceObject["formula"]["reference"].ToString(), FormulaOperator = Constants.FormulaOperators[10] });
                        }
                    }
                }


                if (token.Children().Count() != 0)
                {
                    RecursiveIteration(token.Children(), singleJob);
                }
                else
                {
                    Console.WriteLine("WE ARE AT THE END: ");
                    singleJob.FoundIfFormula = false;
                    singleJob.IsInFormulaObject = false;
                }

            }
            Console.WriteLine("-----------------------------\n\n\n\n\n");

            return singleJob;
        }

        private string ParseFormulaReferences(JArray elements, FormulaOperator formulaOperator)
        {
            var expr = "";
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