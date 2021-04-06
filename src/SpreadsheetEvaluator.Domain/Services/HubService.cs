using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using static SpreadsheetEvaluator.Domain.Services.HubService;

namespace SpreadsheetEvaluator.Domain.Services
{
    // TODO: We may not catch constant values (now we are picking references nicely).
    // TODO: Make this parser more robust
    // TODO: Update/Remove Nuget packages
    // TODO: Clean up code using suggestions from VS
    // TODO: Move classes to folders
    // TODO: Better naming for everything almost
    // TODO: Tests
    // TODO: Fix HttpClientHelper to make the requests
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
    // TODO: Fix structure for [][] (see postman)






    public class HubService : IHubService
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly HttpClientHelper _httpClientHelper;

        public HubService(IOptionsMonitor<ApplicationSettings> configuration, HttpClientHelper httpClientHelper)
        {
            _applicationSettings = configuration.CurrentValue;
            _httpClientHelper = httpClientHelper;
        }

        public class GeneratedFormulaResult
        {
            public string Formula { get; set; }
            public FormulaOperator FormulaOperator { get; set; }
        }


        public class SimpleNode
        {
            public JContainer node { get; set; }
            public bool IsVisited { get; set; }


        }


        public class SingleJob
        {
            public string Id { get; set; }

            public Dictionary<string, SingleValueObject> ReferencesToValues { get; set; } = new Dictionary<string, SingleValueObject>();

            public List<GeneratedFormulaResult> FormulasToReferences { get; set; } = new List<GeneratedFormulaResult>();

            public List<SimpleNode> Nodes { get; set; } = new List<SimpleNode>();


            public bool IteratingOverDataArray { get; set; }
            public bool IteratingOverInsideArray { get; set; }
            public bool IsMultiArray { get; set; }
            public bool foundValue { get; set; }
            public bool HaveEverSeenValue { get; set; }


            // Formulas
            public bool IsInFormulaObject { get; set; }
            public bool FoundIfFormula { get; set; }
            public bool FoundIsGreater { get; set; }


            private int letterIndex = 65;
            private int cellIndex = 1;

            public void SetReferenceToValue(SingleValueObject singleValueObject)
            {
                var placeHolderName = ((char)letterIndex).ToString() + cellIndex.ToString();
                ReferencesToValues[placeHolderName] = singleValueObject;
            }

            public void IncrementCellIndex()
            {
                cellIndex++;
            }

            public void IncrementLetterIndex()
            {
                letterIndex++;
            }

            public void ResetCellIndex()
            {
                cellIndex = 1;
            }

            public void ResetLetterIndex()
            {
                letterIndex = 65;
            }
        }

        public string GetJobs()
        {
            var content = _httpClientHelper.GetStringContentFromUrl(_applicationSettings.HubApiUrlGetJobs);
            //var content = File.ReadAllText(@"C:\Users\Gintautas\Documents\Visual Studio 2019\Projects\Wix-grow-developers-task\src\SpreadsheetEvaluator.Domain\test.json");
            var myObject = JsonConvert.DeserializeObject<JobsRawResponse>(content);

            var ourJobs = new List<SingleJob>();
            foreach (var value in myObject.Jobs.Values.Children())
            {
                Console.WriteLine(" JOB: " + value.ToString());
                var ret = RecursiveIteration(value, new SingleJob());
                ourJobs.Add(ret);
            }


            var computedJobs = CalculationService.Compute(ourJobs);


            var serialized = JsonConvert.SerializeObject(computedJobs,
                            Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            PostJobs(myObject.SubmissionUrl, serialized);

            throw new NotImplementedException();
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

                        singleJob.foundValue = true;
                    }
                    else if (token is JProperty valueObject && singleJob.foundValue)
                    {
                        var t = valueObject.AncestorsAndSelf()
                                               .OfType<JObject>()
                                               .Any(n => n.ContainsKey("formula"));

                        if (t == false)
                        {
                            var newSingleValueObject = new SingleValueObject();
                            singleJob.Nodes.Add(new SimpleNode { node = valueObject, IsVisited = true });

                            if (valueObject.Name == "text")
                            {
                                newSingleValueObject.ResultType = ResultType.Text;
                                newSingleValueObject.Value = valueObject.Value.ToString();
                            }
                            else if (valueObject.Name == "number")
                            {
                                newSingleValueObject.ResultType = ResultType.Number;

                                Decimal.TryParse(valueObject.Value.ToString(), out decimal decimalValue);
                                newSingleValueObject.Value = decimalValue;
                            }
                            else if (valueObject.Name == "boolean")
                            {
                                newSingleValueObject.ResultType = ResultType.Bool;
                                bool.TryParse(valueObject.Value.ToString(), out bool booleanValue);

                                newSingleValueObject.Value = booleanValue;
                            }

                            singleJob.SetReferenceToValue(newSingleValueObject);
                            singleJob.IncrementLetterIndex();
                            singleJob.foundValue = false;
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

                            var formulaOperator = FormulaOperators.Single(x => firstIfObject.ContainsKey(x.JsonName));

                            var elements = firstIfObject[formulaOperator.JsonName] as JArray;
                            var expr = "IIF(" + ParseFormulaReferences(elements, formulaOperator) + "," + secondIfObject["reference"] + "," + thirdIfObject["reference"] + ")";

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty sumFormulaObject && sumFormulaObject.Name == "sum")
                        {
                            var sumArray = sumFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => sumFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty multiplyFormulaObject && multiplyFormulaObject.Name == "multiply")
                        {
                            var sumArray = multiplyFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => multiplyFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty divideFormulaObject && divideFormulaObject.Name == "divide")
                        {
                            var sumArray = divideFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => divideFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty isGreaterFormulaObject && isGreaterFormulaObject.Name == "is_greater" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = isGreaterFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => isGreaterFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty isEqualFormulaObject && isEqualFormulaObject.Name == "is_equal" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = isEqualFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => isEqualFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty notFormulaObject && notFormulaObject.Name == "not" && singleJob.FoundIfFormula == false)
                        {
                            var formulaOperator = FormulaOperators.Single(x => notFormulaObject.Name.Equals(x.JsonName));
                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = "NOT " + GetValueOfAnyType(notFormulaObject), FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty andFormulaObject && andFormulaObject.Name == "and" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = andFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => andFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty orFormulaObject && orFormulaObject.Name == "or" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = orFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => orFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JProperty concatFormulaObject && concatFormulaObject.Name == "concat" && singleJob.FoundIfFormula == false)
                        {
                            var sumArray = concatFormulaObject.Value as JArray;
                            var formulaOperator = FormulaOperators.Single(x => concatFormulaObject.Name.Equals(x.JsonName));
                            var expr = ParseFormulaReferences(sumArray, formulaOperator);

                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = expr, FormulaOperator = formulaOperator });
                        }
                        else if (token is JObject emptyFormulaFieldOnlyReferenceObject && emptyFormulaFieldOnlyReferenceObject.ContainsKey("formula") && emptyFormulaFieldOnlyReferenceObject.ContainsKey("formula") && emptyFormulaFieldOnlyReferenceObject["formula"]["reference"] != null)
                        {
                            singleJob.FormulasToReferences.Add(new GeneratedFormulaResult { Formula = emptyFormulaFieldOnlyReferenceObject["formula"]["reference"].ToString(), FormulaOperator = FormulaOperators[10] });
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
            var elements2 = elements.DeepClone() as JArray;

            var expr = "";
            for (var i = 0; i < elements2.Count; i++)
            {

                if (elements2[i]["reference"] != null)
                {
                    expr += elements2[i]["reference"];
                    
                }
                else if (elements2[i]["value"] != null)
                {
                    var jsonObject = elements2[i]["value"] as JObject;

                    expr += GetValueOfAnyType(jsonObject);
                }

                if (i + 1 < elements2.Count)
                {
                    if (string.IsNullOrEmpty(formulaOperator.MathSymbol) == false)
                    {
                        expr += " " + formulaOperator.MathSymbol + " ";
                    }
                }
                
            }
            elements = null;
            return expr;
        }

        private string GetValueOfAnyType(JObject jObject)
        {
            var obj = jObject.DeepClone() as JObject;

            if (obj.ContainsKey("text"))
            {
                jObject = null;
                return obj["text"].ToString();
            }
            else if (obj.ContainsKey("number"))
            {
                jObject = null;
                return obj["number"].ToString();
            }
            else if (obj.ContainsKey("boolean"))
            {
                jObject = null;
                return obj["boolean"].ToString();
            }
            return "";
        }

        private string GetValueOfAnyType(JProperty jProperty)
        {
            var property = jProperty.DeepClone() as JProperty;


            if (property.Value["reference"] != null)
            {
                jProperty = null;
                return property.Value["reference"].ToString();
            }
            else if (property.Value["text"] != null)
            {
                jProperty = null;
                return property.Value["text"].ToString();
            }
            else if (property.Value["number"] != null)
            {
                jProperty = null;
                return property.Value["number"].ToString();
            }
            else if (property.Value["boolean"] != null)
            {
                jProperty = null;
                return property.Value["boolean"].ToString();
            }
            return "";
        }



        public static List<FormulaOperator> FormulaOperators = new List<FormulaOperator>() {

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











        public void PostJobs(string submissionUrl, string payload)
        {

            _httpClientHelper.PostStringContentToUrl(submissionUrl, payload);


            throw new NotImplementedException();
        }

    }
}
