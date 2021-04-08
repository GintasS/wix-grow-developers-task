namespace SpreadsheetEvaluator.Domain.Models.Enums
{
    public enum ResultType 
    { 
        Number, 
        Text, 
        Bool, 
        IFFormula, 
        Undefined, 
        Reference,
        NotCalculatedYet, 
        Formula
    }
}