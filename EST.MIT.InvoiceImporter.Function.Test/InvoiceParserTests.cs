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
            var csvData = "InvoiceType,AccountType,Organisation,SchemeType,Reference,Created,Updated,CreatedBy,UpdatedBy,SourceSystem," +
                "FRN,MarketingYear,PaymentRequestNumber,AgreementNumber,Currency,DueDate,PaymentRequestValue,LineValue,Description," +
                "SchemeCode,DeliveryBody\r\n" +
                "First, AP,\"Noonans Free range Eggs Ltd\",Scheme1,Reference1,18/04/2023 12:48,18/04/2023 12:48,M186895," +
                "M186895,Manual,1234567890,2022,1,AHWR12345678,GBP,18/04/2023,500,500,\"G00 - Gross value of claim\",12345A,RP00\r\n" +
                "Amendment,AR,\"Nicks Stud Farm\",Scheme2,Reference2,19/04/2023 12:48,19/04/2023 12:48,M186895,M186895,Manual,1234567890," +
                "2023,2,HARW32165487,GBP,18/04/2023,250,250,\"P14 - Cross compliance penalty\",12345A,RP00\r\n" +
                "Amendment, AP,\"Quinton, Dickinson & Sons Pigs\",Scheme2,Reference3,19/04/2023 12:48,19/04/2023 12:48,M186895," +
                "M186895,Manual,1234567890,2023,2,BACON32165487,GBP,18/04/2023,250,250,\"P14 - Cross compliance penalty\",12345A,RP00\r\n";

            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            //Act
            var parsedData = await _invoiceParser.GetInvoicesAsync(csvStream, _mockLogger.Object);

            //Assert
            Assert.True(parsedData != null);
        }

    }
}
