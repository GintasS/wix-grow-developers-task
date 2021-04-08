using SpreadsheetEvaluator.Domain.MathModels;
using System;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Models.MathModels
{
    public class SingleJob
    {
        public string Id { get; set; }
        public List<List<Cell>> Cells { get; set; } = new List<List<Cell>>();
        public bool IteratingOverDataArray { get; set; }
        public bool IteratingOverInsideArray { get; set; }
        public bool IsMultiArray { get; set; }
        public bool FoundValue { get; set; }
        public bool HaveEverSeenValue { get; set; }

        // Formulas
        public bool IsInFormulaObject { get; set; }
        public bool FoundIfFormula { get; set; }

        private int cellLetterIndex = 65;
        private int cellNumberIndex = 1;

        public void SetReferenceToValue(Cell cell)
        {
            cell.Key = ((char)cellLetterIndex).ToString() + cellNumberIndex.ToString();
            var y = cellNumberIndex - 1;
            var x = cellLetterIndex - 65;

            if (y >= Cells.Count)
            {
                var diff = Math.Abs(Cells.Count - (y + 1));
                for (var i = 0; i < diff; i++)
                {
                    Cells.Add(new List<Cell>());
                }
            }

            if (x >= Cells[y].Count)
            {
                var diff = Math.Abs(Cells[y].Count - (x + 1));
                for (var i = 0; i < diff; i++)
                {
                    Cells[y].Add(new Cell());
                }
            }

            Cells[y][x] = cell;
            IncrementLetterIndex();
        }

        public void IncrementCellIndex()
        {
            cellNumberIndex++;
        }

        public void IncrementLetterIndex()
        {
            cellLetterIndex++;
        }

        public void ResetCellIndex()
        {
            cellNumberIndex = 1;
        }

        public void ResetLetterIndex()
        {
            cellLetterIndex = 65;
        }

        public void SetFormula(Formula formula)
        {
            var newCell = new Cell()
            {
                Value = new CellValue(formula),
                Key = ((char)cellLetterIndex).ToString() + cellNumberIndex.ToString()
            };

            var y = cellNumberIndex - 1;
            var x = cellLetterIndex - 65;

            if (y >= Cells.Count)
            {
                var diff = Math.Abs(Cells.Count - (y + 1));
                for (var i = 0; i < diff; i++)
                {
                    Cells.Add(new List<Cell>());
                }
            }

            if (x >= Cells[y].Count)
            {
                var diff = Math.Abs(Cells[y].Count - (x + 1));
                for (var i = 0; i < diff; i++)
                {
                    Cells[y].Add(new Cell());
                }
            }

            Cells[y][x] = newCell;
            IncrementLetterIndex();
        }
    } 
}