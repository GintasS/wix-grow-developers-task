using System;
using SpreadsheetEvaluator.Domain.Extensions;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Enums;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetEvaluator.Domain.Services
{
    public class FormulaEvaluatorService : IFormulaEvaluatorService
    {
        public List<JobComputed> ComputeFormulas(List<JobRaw> jobsList)
        {
            // Map our singleJobs to computed jobs,
            // to not change passed data directly and remove unused properties.
            var computedJobs = jobsList.Select(x => new JobComputed {
                Id = x.Id,
                Cells = x.Cells
            }).ToList();

            foreach(var job in computedJobs)
            {
                foreach (var cellRow in job.Cells)
                {
                    // Reverse cellRow if the value is at the end and formulas are at the start.
                    var isCellRowInvertedNormalCellLastIndex = cellRow.FindLastIndex(x => x.Value.CellType != CellType.Formula);
                    var isCellRowInvertedFormulaCellFirstIndex = cellRow.FindIndex(x => x.Value.CellType == CellType.Formula);

                    if (isCellRowInvertedNormalCellLastIndex > isCellRowInvertedFormulaCellFirstIndex)
                    {
                        cellRow.Reverse();
                    }

                    foreach (var individualCell in cellRow)
                    {
                        var formula = individualCell.Value.Value as Formula;
                        if (formula == null)
                        {
                            continue;
                        }

                        // If the formula of a reference type, find a value that this reference points to.
                        if (formula.FormulaOperator.FormulaResultType == FormulaResultType.Reference)
                        {
                            var referencedCell = job.Cells.SelectMany(x => x)
                                .FirstOrDefault(x => x.Key == formula.Text);

                            if (referencedCell != null)
                            {
                                individualCell.Value.UpdateCell(referencedCell.Value);
                            }
                        }
                        else
                        {
                            // Filter cells without formulas.
                            var cellsWithoutFormulas = cellRow.Where(x => x.Value.IsFormulaCell == false)
                                .ToList();

                            // Check if our values that are not yet replaced in the formula ("A1 + B1")
                            // are of a mismatching type.
                            var hasMismatchingElementTypes = cellsWithoutFormulas.Where(x => formula.Text.IndexOf(x.Key, StringComparison.Ordinal) >= 0)
                                .Select(x => x.Value.CellType)
                                .ToList()
                                .HasMismatchingElementTypes();

                            if (hasMismatchingElementTypes)
                            {
                                individualCell.Value.SetCellAsErrorCell();
                                continue;
                            }

                            // Replace references in the formula.
                            // E.g "A1 + B1" is going to be "5 + 4" after this method execution.
                            var mathExpressionText = CalculationHelper.ReplaceFormulaReferencesWithValues(formula.Text, cellsWithoutFormulas);
                            
                            // Replace cell's value with the math expression, which is a string currently.
                            individualCell.Value.UpdateCell(mathExpressionText);

                            // Compute math expression.
                            // Do not compute value for formula with a string type,
                            // because we will get an exception.

                            var computationResult = individualCell.Value.Value;
                            if (formula.FormulaOperator.FormulaResultType != FormulaResultType.Text)
                            {
                                computationResult = computationResult.CalculateMathExpression();
                            }

                            // If we got a null, set this cell as an error cell.
                            if (computationResult == null)
                            {
                                individualCell.Value.SetCellAsErrorCell();
                                continue;
                            }

                            // Update the cell with the computed value.
                            individualCell.Value.UpdateCell((dynamic)computationResult);
                        }
                        
                    }                 
                } 
            }
            return computedJobs;
        }
    }
}