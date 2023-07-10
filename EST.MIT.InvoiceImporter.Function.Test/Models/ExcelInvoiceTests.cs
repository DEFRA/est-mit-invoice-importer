using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class ExcelInvoiceTests
{
    [Fact]
    public void TestExcelInvoiceProperties()
    {
        var excelInvoice = new ExcelInvoice
        {
            Id = "TestId",
            InvoiceType = "TestInvoiceType",
            AccountType = "TestAccountType",
            Organisation = "TestOrganisation",
            SchemeType = "TestSchemeType"
        };

        Assert.Equal("TestId", excelInvoice.Id);
        Assert.Equal("TestInvoiceType", excelInvoice.InvoiceType);
        Assert.Equal("TestAccountType", excelInvoice.AccountType);
        Assert.Equal("TestOrganisation", excelInvoice.Organisation);
        Assert.Equal("TestSchemeType", excelInvoice.SchemeType);
    }
}