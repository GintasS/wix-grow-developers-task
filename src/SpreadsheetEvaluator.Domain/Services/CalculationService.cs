using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static SpreadsheetEvaluator.Domain.Services.HubService;

namespace SpreadsheetEvaluator.Domain.Services
{
    public class CalculationService
    {
        public static JobsPostRequest Compute(List<SingleJob> ourJobs)
        {
            var jobsPostRequest = new JobsPostRequest
            {
                Email = "gintautas@gsvedas.com",
                Jobs = new List<JobsPostModel>()
            };

            var jobs = ourJobs.Select(x => new {
                Id = x.Id,
                References = x.ReferencesToValues,
                Formulas = x.FormulasToReferences
            }).ToList();





            foreach (var job in jobs)
            {
                var valueMismatchIsFound = false;


                var jobsPostModel = new JobsPostModel()
                {
                    Id = job.Id,
                    Values = new List<List<JobsPostValueModel>>() { }
                };

                if (job.References.Count() != 0)
                {
                    jobsPostModel.Values.Add(new List<JobsPostValueModel>());
                }



                foreach (var reference in job.References)
                {
                    var jobsPostValueModel = new JobsPostValueModel()
                    {
                        Values = new Values()
                    };

                    if (reference.Value.ResultType == ResultType.Number)
                    {
                        jobsPostValueModel.Values.number = (decimal)reference.Value.Value;
                    }
                    if (reference.Value.ResultType == ResultType.Text)
                    {
                        jobsPostValueModel.Values.text = (string)reference.Value.Value;
                    }
                    if (reference.Value.ResultType == ResultType.Bool)
                    {
                        jobsPostValueModel.Values.boolean = (bool)reference.Value.Value;
                    }


                    Int32.TryParse(reference.Key.Substring(1, 1), out int index);
                    index--;

                    if (index >= jobsPostModel.Values.Count)
                    {
                        jobsPostModel.Values.Add(new List<JobsPostValueModel>());
                    }

                    jobsPostModel.Values[index].Add(jobsPostValueModel);
                }


                foreach (var formula in job.Formulas)
                {
                    var singleValueObject = new SingleValueObject()
                    {
                        Value = formula.Formula,
                        ResultType = ResultType.NotCalculatedYet
                    };

                    var jobsPostValueModelFormulas = new JobsPostValueModel()
                    {
                        Values = new Values()

                    };

                    var columNumber = "";

                    var allReferenceTypes = new List<ResultType>();
                    foreach (var referenceReplace in job.References)
                    {
                        if (formula.FormulaOperator.FormulaResultType == ResultType.Reference)
                        {
                            singleValueObject.ResultType = referenceReplace.Value.ResultType;
                        }
                        else
                        {
                            singleValueObject.ResultType = formula.FormulaOperator.FormulaResultType;
                        }



                        if (singleValueObject.Value.ToString().IndexOf(referenceReplace.Key) >= 0)
                        {
                            allReferenceTypes.Add(referenceReplace.Value.ResultType);
                        }


                        if (singleValueObject.Value.ToString().IndexOf(referenceReplace.Key) >= 0 && string.IsNullOrEmpty(referenceReplace.Key) == false)
                        {
                            columNumber = referenceReplace.Key;
                        }

                        var stringg = singleValueObject.Value.ToString().Replace(referenceReplace.Key, referenceReplace.Value.Value.ToString());

                        singleValueObject.Value = stringg;
                    }

                    if (singleValueObject.ResultType == ResultType.NotCalculatedYet)
                    {
                        singleValueObject.ResultType = ResultType.Text;
                    }

                    if (job.Id == "job-18")
                    {
                        Console.WriteLine("A");
                    }

                    object computationResult = singleValueObject.Value;
                    try
                    {
                        if (singleValueObject.ResultType != ResultType.Text)
                        {
                            computationResult = new DataTable().Compute(singleValueObject.Value.ToString(), null);
                        }

                        if (IsListUnique(allReferenceTypes) == false)
                        {
                            throw new Exception();
                        }


                    }
                    catch (Exception ex)
                    {
                        jobsPostValueModelFormulas.error = "type does not match";
                        jobsPostValueModelFormulas.Values = null;
                    }




                    if (jobsPostValueModelFormulas.error == null)
                    {
                        if (computationResult.IsNumber())
                        {
                            singleValueObject.ResultType = ResultType.Number;
                        }
                        else if (computationResult is string)
                        {
                            singleValueObject.ResultType = ResultType.Text;
                        }
                        else if (computationResult is bool)
                        {
                            singleValueObject.ResultType = ResultType.Bool;
                        }

                        if (singleValueObject.ResultType == ResultType.Number)
                        {
                            Decimal.TryParse(computationResult.ToString(), out decimal dec);
                            jobsPostValueModelFormulas.Values.number = dec;

                        }
                        if (singleValueObject.ResultType == ResultType.Text)
                        {
                            jobsPostValueModelFormulas.Values.text = computationResult.ToString();
                        }
                        if (singleValueObject.ResultType == ResultType.Bool)
                        {
                            jobsPostValueModelFormulas.Values.boolean = (bool)computationResult;
                        }
                    }


                    int index = 0;

                    if (string.IsNullOrEmpty(columNumber) == false)
                    {
                        Int32.TryParse(columNumber.Substring(1, 1), out index);
                        index--;

                        if (index >= jobsPostModel.Values.Count)
                        {
                            jobsPostModel.Values.Add(new List<JobsPostValueModel>());
                        }
                    }

                    if (jobsPostModel.Values.Count() == 0)
                    {
                        jobsPostModel.Values.Add(new List<JobsPostValueModel>());
                    }

                    jobsPostModel.Values[index].Add(jobsPostValueModelFormulas);
                }


                jobsPostRequest.Jobs.Add(jobsPostModel);
            }


            return jobsPostRequest;
        }

        private static bool IsListUnique(List<ResultType> list)
        {
            if (list == null || list.Count <= 1)
            {
                return true;
            }


            ResultType firsType = list[0];

            for (var i = 1; i < list.Count; i++)
            {
                if ((int)list[i] != (int)firsType)
                {
                    return false;
                }

                firsType = list[i];
            }



            return true;
        }
    }
}
