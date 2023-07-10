using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class ExcelHeaderTests
{
    [Fact]
    public void TestExcelHeaderProperties()
    {
        var excelHeader = new ExcelHeader
        {
            PaymentRequestId = "TestId",
            AgreementNumber = "TestAgreementNumber",
            ClaimRef = "TestClaimRef",
            FRN = "107377217",
            PaymentRequestNumber = "0",
            ContractNumber = "TestContractNumber",
            Value = "2613.69",
            DeliveryBody = "TestDeliveryBody",
            DueDate = "TestDueDate",
            RecoveryDate = "TestRecoveryDate",
            Description = "TestDescription"
        };

        Assert.Equal("TestId", excelHeader.PaymentRequestId);
        Assert.Equal("TestAgreementNumber", excelHeader.AgreementNumber);
        Assert.Equal("TestClaimRef", excelHeader.ClaimRef);
        Assert.Equal("107377217", excelHeader.FRN);
        Assert.Equal("0", excelHeader.PaymentRequestNumber);
        Assert.Equal("TestContractNumber", excelHeader.ContractNumber);
        Assert.Equal("2613.69", excelHeader.Value);
        Assert.Equal("TestDeliveryBody", excelHeader.DeliveryBody);
        Assert.Equal("TestDueDate", excelHeader.DueDate);
        Assert.Equal("TestRecoveryDate", excelHeader.RecoveryDate);
        Assert.Equal("TestDescription", excelHeader.Description);
    }
}