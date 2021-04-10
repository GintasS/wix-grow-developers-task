using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Newtonsoft.Json;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Enums;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Models.Responses;
using SpreadsheetEvaluator.Domain.Services;
using Xunit;

namespace SpreadsheetEvaluator.UnitTests.Services
{
    public class SpreadsheetCreationServiceTests
    {
        private static List<JobRaw> _jobsRawExpected = new List<JobRaw>
        {
            new JobRaw
            {
                Id = "job-0",
                Cells = new List<List<Cell>>
                {
                    new List<Cell>()
                }
            },
            new JobRaw
            {
                Id = "job-1",
                Cells = new List<List<Cell>>
                {
                    new List<Cell>
                    {
                        new Cell
                        {
                            Key = "A1",
                            Value = new CellValue(5)
                        }
                    }
                }
            },
            new JobRaw
            {
                Id = "job-2",
                Cells = new List<List<Cell>>
                {
                    new List<Cell>
                    {
                        new Cell
                        {
                            Key = "A1",
                            Value = new CellValue(5)
                        },
                        new Cell
                        {
                            Key = "B1",
                            Value = new CellValue(new Formula
                            {
                                Text = "A1",
                                FormulaOperator = Constants.FormulaOperators[10]
                            })
                        },
                    }
                }
            },
            new JobRaw
            {
                Id = "job-3",
                Cells = new List<List<Cell>>
                {
                    new List<Cell>
                    {
                        new Cell
                        {
                            Key = "A1",
                            Value = new CellValue(6)
                        },
                        new Cell
                        {
                            Key = "B1",
                            Value = new CellValue(4)
                        },
                        new Cell
                        {
                            Key = "C1",
                            Value = new CellValue(new Formula
                            {
                                Text = "A1 + B1",
                                FormulaOperator = Constants.FormulaOperators[0]
                            })
                        },

                    }
                }
            },
            new JobRaw
            {
                Id = "job-4",
                Cells = new List<List<Cell>>
                {
                    new List<Cell>
                    {
                        new Cell
                        {
                            Key = "A1",
                            Value = new CellValue(6)
                        },
                        new Cell
                        {
                            Key = "B1",
                            Value = new CellValue(4)
                        },
                        new Cell
                        {
                            Key = "C1",
                            Value = new CellValue(7)
                        },
                        new Cell
                        {
                            Key = "D1",
                            Value = new CellValue(new Formula
                            {
                                Text = "A1 + B1 + C1",
                                FormulaOperator = Constants.FormulaOperators[0]
                            })
                        },
                    }
                }
            },

        };

        private readonly ISpreadsheetCreationService _spreadsheetCreationService;

        public SpreadsheetCreationServiceTests()
        {
            _spreadsheetCreationService = new SpreadsheetCreationService();
        }

        [Fact]
        public void Should_Compute_Formulas_For_Jobs_Raw()
        {
            // Arrange
            var jsonRaw = ResourceFileReader.GetString("SpreadsheetEvaluator.UnitTests.Resources.JobsRaw.json");
            var expectedJobsGetRawResponse = JsonConvert.DeserializeObject<JobsGetRawResponse>(jsonRaw);

            // Act
            var actualJobsRaw = _spreadsheetCreationService.Create(expectedJobsGetRawResponse);

            // Assert
            actualJobsRaw.Should().NotBeNull();
            actualJobsRaw.Count.Should().Be(5);

            for (var i = 0; i < 5; i++)
            {
                actualJobsRaw[i].Id.Should().Be(_jobsRawExpected[i].Id);

                if (actualJobsRaw[i].Cells.Count == 0)
                {
                    continue;
                }

                var cellsCount = actualJobsRaw[i].Cells[0].Count;
                for (var y = 0; y < cellsCount; y++)
                {
                    actualJobsRaw[i].Cells[0][y].Value.CellType.Should().Be(_jobsRawExpected[i].Cells[0][y].Value.CellType);
                    
                    if (actualJobsRaw[i].Cells[0][y].Value.CellType.Equals(CellType.Formula))
                    {
                        var actualJobsRawFormula = actualJobsRaw[i].Cells[0][y].Value.Value as Formula;
                        var expectedJobsRawFormula = _jobsRawExpected[i].Cells[0][y].Value.Value as Formula;

                        if (actualJobsRawFormula != null && expectedJobsRawFormula != null)
                        {
                            actualJobsRawFormula.FormulaOperator.Should().Be(expectedJobsRawFormula.FormulaOperator);
                            actualJobsRawFormula.Text.Should().Be(expectedJobsRawFormula.Text);
                        }

                        continue;
                    }

                    actualJobsRaw[i].Cells[0][y].Value.Value.Should().Be(_jobsRawExpected[i].Cells[0][y].Value.Value);
                }
            }
        }
    }
}
