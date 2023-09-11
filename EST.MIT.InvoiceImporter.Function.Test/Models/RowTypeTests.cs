using EST.MIT.InvoiceImporter.Function.Models;
using Xunit;

namespace EST.MIT.InvoiceImporter.Function.Test.Models
{
    public class ExcelRowTypeTests
    {
        [Fact]
        public void ExcelRowType_EnumValues_AreCorrect()
        {
            var expectedValues = new[] { "InvoiceHeaderLine", "HeaderLine", "Line", "Undefined" };

            var actualValues = Enum.GetNames(typeof(ExcelRowType));

            Assert.Equal(expectedValues, actualValues);
        }

        [Theory]
        [InlineData(ExcelRowType.InvoiceHeaderLine, 0)]
        [InlineData(ExcelRowType.HeaderLine, 1)]
        [InlineData(ExcelRowType.Line, 2)]
        [InlineData(ExcelRowType.Undefined, 3)]
        public void ExcelRowType_EnumValues_HaveCorrectIndices(ExcelRowType rowType, int expectedIndex)
        {
            Assert.Equal(expectedIndex, (int)rowType);
        }
    }
}
