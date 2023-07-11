using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class ExcelLineTests
{
    [Fact]
    public void TestExcelLineProperties()
    {
        var excelLine = new ExcelLine
        {
            Value = "2613.69",
            Currency = "GBP",
            FundCode = "TestFundCode",
            MainAccount = "TestMainAccount",
            SchemeCode = "TestSchemeCode",
            MarketingYear = "2014",
            Description = "TestDescription"
        };

        Assert.Equal("2613.69", excelLine.Value);
        Assert.Equal("GBP", excelLine.Currency);
        Assert.Equal("TestFundCode", excelLine.FundCode);
        Assert.Equal("TestMainAccount", excelLine.MainAccount);
        Assert.Equal("TestSchemeCode", excelLine.SchemeCode);
        Assert.Equal("2014", excelLine.MarketingYear);
        Assert.Equal("TestDescription", excelLine.Description);
    }
}