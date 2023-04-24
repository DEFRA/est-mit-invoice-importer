using InvoiceImporter.Function.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace EST.MIT.InvoiceImporter.Function.Test
{
    public class InvoiceParserTests
    {
        private static IInvoiceParser? _invoiceParser;
        private readonly Mock<ILogger> _mockLogger;

        public InvoiceParserTests()
        {
            _mockLogger = new Mock<ILogger>();
            _invoiceParser = new InvoiceParser();
        }

        [Fact]
        public async Task ReadCSVIntoObject_ReturnsData()
        {
            //Arrange
            var csvData = "InvoiceType,AccountType,Organisation,SchemeType,Reference,Created,Updated,CreatedBy,UpdatedBy\r\n" +
                          "First ,AP,\"Noonans Free range Eggs Ltd\",Scheme1,Reference1,18/04/2023 12:48,18/04/2023 12:48,M186895,M186895\r\n" +
                          "Amendment,AR,\"Nicks Cow farm\",Scheme2,Reference2,19/04/2023 12:48,19/04/2023 12:48,M186895,M186895\r\n";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            //Act
            var parsedData = await _invoiceParser.GetInvoicesAsync(csvStream, _mockLogger.Object);

            //Assert
            Assert.True(parsedData != null);
        }

    }
}
