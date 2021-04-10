using FluentAssertions;
using SpreadsheetEvaluator.Domain.Configuration;
using SpreadsheetEvaluator.Domain.Models.MathModels;
using System.Collections.Generic;
using SpreadsheetEvaluator.Domain.Models.Enums;
using Xunit;
using System;
using Microsoft.CSharp.RuntimeBinder;

namespace SpreadsheetEvaluator.UnitTests.Models.MathModels
{
    public class CellValueTests
    {
        public static IEnumerable<object[]> DataCreateCell => new List<object[]>
        {
            new object[] { 0, CellType.Number, 0 },
            new object[] { true, CellType.Boolean, true }
        };

        public static IEnumerable<object[]> DataCreateCellStringDecimal = new List<object[]>
        {
            new object[] { "30.50", CellType.Number, "30.50", true},
            new object[] { "test", CellType.Text, "test", false}
        };

        [Theory]
        [MemberData(nameof(DataCreateCell))]
        public void Should_Create_Cell_Value(object expectedValue, CellType expectedCellType, object value)
        {
            // Act
            var cellValue = new CellValue(value);

            // Assert
            cellValue.Value.Should().Be(expectedValue);
            cellValue.CellType.Should().Be(expectedCellType);
        }

        [Theory]
        [MemberData(nameof(DataCreateCellStringDecimal))]
        public void Should_Create_Cell_Value_For_Decimal_Value(object expectedValue, CellType expectedCellType, object value, bool isDecimalValue = false, bool isStringValue = false)
        {
            // Arrange
            if (isDecimalValue)
            {
                decimal.TryParse(value.ToString(), out var givenDecimalResult);
                decimal.TryParse(value.ToString(), out var expectedDecimalResult);
                value = givenDecimalResult;
                expectedValue = expectedDecimalResult;
            }

            // Act
            var cellValue = new CellValue(value);

            // Assert
            cellValue.Value.Should().Be(expectedValue);
            cellValue.CellType.Should().Be(expectedCellType);

            if (isStringValue)
            {
                cellValue.Value.Should().NotBeSameAs(expectedValue);
            }
        }

        [Fact]
        public void Should_Create_Cell_Value_With_Formula_Object_Value()
        {
            // Arrange
            var formula = new Formula
            {
                FormulaOperator = Constants.FormulaOperators[0],
                Text = "A1"
            };

            // Act
            var cellValue = new CellValue(formula);

            // Assert
            var formulaValueFromCell = cellValue.Value as Formula;
            formulaValueFromCell.Should().NotBe(null);
            formulaValueFromCell.Should().NotBeSameAs(formula);
            formulaValueFromCell.FormulaOperator.Should().Be(formula.FormulaOperator);
            formulaValueFromCell.Text.Should().Be(formula.Text);
            cellValue.Value.Should().BeOfType<Formula>();
            cellValue.CellType.Should().Be(CellType.Formula);
        }

        [Fact]
        public void Should_Create_Cell_Value_With_Cell_Value_Object()
        {
            // Arrange
            var originalCellValue = new CellValue("test");

            // Act
            var cellValueToBeCreated = new CellValue(originalCellValue);

            // Assert
            cellValueToBeCreated.Should().NotBe(null);
            cellValueToBeCreated.Should().NotBe(originalCellValue);
            cellValueToBeCreated.Value.Should().Be("test");
            cellValueToBeCreated.CellType.Should().Be(CellType.Text);
            cellValueToBeCreated.Value.Should().NotBeSameAs(originalCellValue.Value);
        }

        [Fact]
        public void Should_Set_Cell_Value_To_Error()
        {
            // Arrange
            var cellValue = new CellValue("test");

            // Act
            cellValue.SetCellAsErrorCell();

            // Assert
            cellValue.Value.Should().Be(Constants.Error.MismatchingTypes);
            cellValue.CellType.Should().Be(CellType.Error);
        }

        [Fact]
        public void Should_Get_Exception_When_InvalidCellType()
        {
            // Act
            Action action = () => new CellValue(new JobRaw());

            // Assert
            action.Should().Throw<RuntimeBinderException>();
        }

    }
}