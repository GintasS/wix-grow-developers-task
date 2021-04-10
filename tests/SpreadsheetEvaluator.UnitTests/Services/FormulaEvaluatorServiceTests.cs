using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Models.Enums;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using SpreadsheetEvaluator.Domain.Services;
using Xunit;

namespace SpreadsheetEvaluator.UnitTests.Services
{
    public class FormulaEvaluatorServiceTests
    {
        private static List<JobRaw> jobsRawGiven = new List<JobRaw>
        {
            new JobRaw
            {
                Id = "job-0",
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
                            Value = new CellValue(10)
                        },
                        new Cell
                        {
                            Key = "C1",
                            Value = new CellValue(new Formula
                            {
                                FormulaOperator = Constants.FormulaOperators[0],
                                Text = "A1 + B1"
                            })
                        }
                    }
                }
            }
        };

        private static List<JobComputed> JobsComputedExpected = new List<JobComputed>
        {
            new JobComputed
            {
                Id = "job-0",
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
                            Value = new CellValue(10)
                        },
                        new Cell
                        {
                            Key = "C1",
                            Value = new CellValue(15)
                        }
                    }
                }
            }
        };

        private readonly IFormulaEvaluatorService _formulaEvaluatorService;

        public FormulaEvaluatorServiceTests()
        {
            _formulaEvaluatorService = new FormulaEvaluatorService();
        }

        [Fact]
        public void Should_Compute_Formulas_For_Jobs_Raw()
        {
            // Act
            var actual = _formulaEvaluatorService.ComputeFormulas(jobsRawGiven);
            
            var jobsComputedExpected = JobsComputedExpected[0].Cells[0]
                .Select(x => x)
                .ToList();

            var actualComputedJobs = actual[0].Cells[0]
                .Select(x => x)
                .ToList();

            // Assert
            actual[0].Id.Should().Be(JobsComputedExpected[0].Id);

            for (var i = 0; i < 3;i++)
            {
                actualComputedJobs[i].Should().NotBeSameAs(jobsComputedExpected[i]);
                actualComputedJobs[i].Value.Value.Should().Be(jobsComputedExpected[i].Value.Value);
                actualComputedJobs[i].Key.Should().Be(jobsComputedExpected[i].Key);
            }
        }
    }
}
