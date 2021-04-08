using Microsoft.Extensions.Options;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Extensions;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Enums;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SpreadsheetEvaluator.Domain.Services
{
    public class FormulaEvaluatorService : IFormulaEvaluatorService
    {
        private readonly ApplicationSettings _applicationSettings;
        
        public FormulaEvaluatorService(IOptionsMonitor<ApplicationSettings> configuration)
        {
            _applicationSettings = configuration.CurrentValue;
        }

        public List<ComputedJob> ComputeFormulas(List<SingleJob> jobsList)
        {
            var computedJobs = jobsList.Select(x => new ComputedJob {
                Id = x.Id,
                Cells = x.Cells
            }).ToList();

            for(var jobIndex = 0; jobIndex < computedJobs.Count; jobIndex++)
            {
                /*
                var isCellRowInvertedNormalCellLastIndex = jobs[jobIndex].Cells.SelectMany(x => x).FirstOrDefault(x => x.Value.CellType != CellType.Formula);
                var isCellRowInvertedFormulaCellFirstIndex = jobs[jobIndex].Cells.FindIndex(x => x.Value.CellType == CellType.Formula);
                */

                for (var cellRowIndex = 0; cellRowIndex < computedJobs[jobIndex].Cells.Count; cellRowIndex++)
                {
                    var cellRow = computedJobs[jobIndex].Cells[cellRowIndex];

                    var isCellRowInvertedNormalCellLastIndex = computedJobs[jobIndex].Cells[cellRowIndex].FindLastIndex(x => x.Value.CellType != CellType.Formula);
                    var isCellRowInvertedFormulaCellFirstIndex = computedJobs[jobIndex].Cells[cellRowIndex].FindIndex(x => x.Value.CellType == CellType.Formula);

                    if (isCellRowInvertedNormalCellLastIndex > isCellRowInvertedFormulaCellFirstIndex)
                    {
                        computedJobs[jobIndex].Cells[cellRowIndex].Reverse();
                    }

                    for (var cellIndex = 0; cellIndex < computedJobs[jobIndex].Cells[cellRowIndex].Count(); cellIndex++)
                    {
                        var individualCell = computedJobs[jobIndex].Cells[cellRowIndex][cellIndex];

                        if (individualCell.Value.Value is Formula formula) 
                        {
                            if (formula.FormulaOperator.FormulaResultType == FormulaResultType.Reference)
                            {
                                var referencedCell = computedJobs[jobIndex].Cells.SelectMany(x => x).FirstOrDefault(x => x.Key == formula.FormulaText);

                                if (referencedCell != null)
                                {
                                    individualCell.Value.TryUpdateCell(referencedCell.Value);
                                }
                            }
                            else
                            {
                                // Replacing References with values from the cells.

                                var cellsWithoutFormulas = cellRow.Where(x => x.Value.IsFormulaCell == false)
                                    .ToList();

                                var hasUniqueElements = cellsWithoutFormulas.Where(x => formula.FormulaText.IndexOf(x.Key) >= 0)
                                    .Select(x => x.Value.CellType)
                                    .ToList()
                                    .HasUniqueElements();

                                formula.ReplaceFormulaReferencesWithValues(cellsWithoutFormulas);
                                individualCell.Value.TryUpdateCell(formula.FormulaText);

                                var computationResult = individualCell.Value.Value;
                                try
                                {
                                    if (formula.FormulaOperator.FormulaResultType != FormulaResultType.Text)
                                    {
                                        computationResult = new DataTable().Compute(computationResult.ToString(), null);
                                    }

                                    if (hasUniqueElements == false)
                                    {
                                        throw new Exception();
                                    }
                                }
                                catch
                                {
                                    individualCell.Value.SetCellAsErrorCell();
                                    continue;
                                }

                                individualCell.Value.TryUpdateCell(computationResult);
                            }
                        }
                    }                 
                } 
            }

            return computedJobs;
        }
    }
}
