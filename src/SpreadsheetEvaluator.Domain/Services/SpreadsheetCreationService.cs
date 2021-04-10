using Newtonsoft.Json.Linq;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetEvaluator.Domain.Services
{
    // TODO: We may not catch constant values (now we are picking references nicely).
    // TODO: Make this parser more robust
    // TODO: Clean up code using suggestions from VS
    // TODO: Tests
    // TODO: Find edge cases to break your code
    // TODO: Validation everywhere
    // TODO: Add something to changelog.
    // TODO: Exception handling where needed
    // TODO: If formula comes first, things can break and we will not save value.
    // TODO: Run Resharper - Inspection - and fix spots.
    public class SpreadsheetCreationService : ISpreadsheetCreationService
    {
        public List<JobRaw> Create(JobsRawResponse jobsRawResponse)
        {
            var createdJobs = new List<JobRaw>();
            var jobNodes = jobsRawResponse.Jobs.Values.Children();
            foreach (var jobNode in jobNodes)
            {
                var createdSingleJob = ReadTokensRecursively(jobNode, new JobRaw());
                createdJobs.Add(createdSingleJob);
            }

            return createdJobs;
        }

        private JobRaw ReadTokensRecursively(IEnumerable<JToken> tokens, JobRaw jobRaw)
        {
            foreach (var token in tokens)
            {
                // We are at the 1 below root, aka, on on the individual job.

                // Get Job Id here.
                if (token is JProperty jobIdProperty && jobIdProperty.Name == Constants.JobJson.Id)
                {
                    jobRaw.Id = jobIdProperty.Value.ToString();
                }
                else if (token?.Parent != null && token?.Parent is JProperty dataArrayProperty && dataArrayProperty.Name == Constants.JobJson.Data && jobRaw.IteratingOverDataArray == false)
                {
                    // In this place, we say that we have found our data array.
                    jobRaw.IteratingOverDataArray = true;
                }

                // In this block we are going both save values and formulas.
                if (jobRaw.IteratingOverDataArray)
                {
                    // Detect multidimensional array.
                    if (token is JArray && token?.Parent is JArray && token.Parent.Count > 1)
                    {
                        if (jobRaw.IsMultiArray == false && token.Parent.Count > 1)
                        {
                            jobRaw.IsMultiArray = true;
                        }
                        else if (jobRaw.IsMultiArray)
                        {
                            jobRaw.ResetLetterIndex();
                            jobRaw.IncrementCellIndex();
                        }
                    }

                    // In this place, we say that we have found formula object.
                    if (token is JObject formulaObject && formulaObject.ContainsKey(Constants.JobJson.Formula) && jobRaw.IsInFormulaObject == false)
                    {
                        jobRaw.IsInFormulaObject = true;
                    }

                    if (token is JProperty && token?.Parent is JObject firtValuePropertyParent && firtValuePropertyParent.ContainsKey(Constants.JobJson.Value))
                    {
                        jobRaw.FoundValue = true;
                    }
                    else if (token is JProperty valueProperty && jobRaw.FoundValue)
                    {
                        // Get simple values here.

                        var isInsideFormulaObject = valueProperty.AncestorsAndSelf()
                            .OfType<JObject>()
                            .Any(n => n.ContainsKey(Constants.JobJson.Formula));

                        if (isInsideFormulaObject == false)
                        {
                            var concreteValue = JsonObjectHelper.GetConcreteValueFromProperty(valueProperty);

                            if (concreteValue != null)
                            {
                                jobRaw.SetCellValue(concreteValue);
                                jobRaw.FoundValue = false;
                            }
                        }
                    }
                    else if (token is JProperty formulaProperty && jobRaw.IsInFormulaObject)
                    {
                        if (formulaProperty.Name == Constants.JobJson.IfValue && jobRaw.FoundIfFormula == false)
                        {
                            jobRaw.FoundIfFormula = true;
                        }
                    }

                    // Get formulas here.
                    if (jobRaw.IsInFormulaObject)
                    {
                        if (token is JArray ifObjectArray && ifObjectArray.Count == 3 && jobRaw.FoundIfFormula)
                        {
                            var ifFormula = FormulaHelper.CreateIfFormula(ifObjectArray);
                            jobRaw.SetCellValue(ifFormula);
                        }
                        else if (token is JProperty concatFormulaProperty && concatFormulaProperty.Name == Constants.Formula.Concat && jobRaw.FoundIfFormula == false)
                        {
                            var concatFormula = FormulaHelper.CreateStandardFormula(concatFormulaProperty);
                            jobRaw.SetCellValue(concatFormula);
                        }
                        else if (token is JProperty standardFormulaProperty && Constants.StandardOperatorNames.Exists(x => x.Equals(standardFormulaProperty.Name)))
                        {
                            var standardFormula = FormulaHelper.CreateStandardFormula(standardFormulaProperty);
                            jobRaw.SetCellValue(standardFormula);
                        }
                        else if (token is JProperty notFormulaProperty && notFormulaProperty.Name == Constants.Formula.Not && jobRaw.FoundIfFormula == false)
                        {
                            var notFormula = FormulaHelper.CreateNotOperatorFormula(notFormulaProperty);
                            jobRaw.SetCellValue(notFormula);
                        }
                        else if (token is JProperty logicalFormulaProperty && Constants.LogicalOperatorNames.Exists(x => x.Equals(logicalFormulaProperty.Name)) && jobRaw.FoundIfFormula == false)
                        {
                            var logicalFormula = FormulaHelper.CreateLogicalOperatorsFormula(logicalFormulaProperty);
                            jobRaw.SetCellValue(logicalFormula);
                        }
                        else if (token is JObject formulaReferenceTypeObject && formulaReferenceTypeObject.ContainsKey(Constants.JobJson.Formula) && formulaReferenceTypeObject[Constants.JobJson.Formula][Constants.JobJson.Reference] != null)
                        {
                            var referenceFormula = FormulaHelper.CreateReferenceFormula(formulaReferenceTypeObject);
                            jobRaw.SetCellValue(referenceFormula);
                        }
                    }
                }

                if (token.Children().Count() != 0)
                {
                    ReadTokensRecursively(token.Children(), jobRaw);
                }
                else
                {
                    jobRaw.FoundIfFormula = false;
                    jobRaw.IsInFormulaObject = false;
                }
            }
            
            return jobRaw;
        }
    }
}