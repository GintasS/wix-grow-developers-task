using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetEvaluator.Domain.Models.Responses
{
    public class JobsRawResponse
    {
        public string SubmissionUrl { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JToken> Jobs { get; set; }
    }




    /*
    public class JobsResponse
    {
        public string SubmissionUrl { get; set; }
        public List<List<ValueResultObject>> Values { get;set; }
        
    }

    public class ValueResultObject
    {
        public ResultType ResultType { get; set; }
        public object Result { get; set; }
    }

    /*

    public class SingleJobObject
    {
        public string Id { get; set; }
        public SingleDataObject Data { get; set; }
    }

    public class SingleDataObject
    {
        public List<SingleValueObject> Values { get; set; }
        public List<SingleFormualObject> Formulas { get; set; }
    }

    public class SingleValueObject
    {
        public decimal? Number { get; set; }
        public bool? Boolean { get; set; }
        public string Text { get; set; }
        public string Error { get; set; }
    }

    public class SingleFormualObject
    {
        public FormulaOperator FormulaOperator { get; set; }
        public List<string> References { get; set; }

    }

    
    */

    public class SingleValueObject
    {
        public ResultType ResultType { get; set; }
        public object Value { get; set; }
    }


    public enum ResultType { Number, Text, Bool, IFFormula, Undefined, Reference,
        NotCalculatedYet
    }
    public enum FormulaOperatorType { sum, multiply, divide, is_greater, is_equal, not, and, or, iff, concat, undefined }

    public class FormulaOperator
    {
        public string JsonName { get; set; }
        public string MathSymbol { get; set; }
        public FormulaOperatorType FormulaOperatorType { get; set; }
        public ResultType FormulaResultType { get; set; }
    }




}
