using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Models.MathModels
{
    public class ComputedJob
    {
        public string Id { get; set; }

        public List<List<Cell>> Cells { get; set; }
    }
}
