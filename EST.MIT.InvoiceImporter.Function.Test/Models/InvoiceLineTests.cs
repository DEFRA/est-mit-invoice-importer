using EST.MIT.InvoiceImporter.Function.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class InvoiceLineTests
{
    [Fact]
    public void TestInvoiceLineProperties()
    {
        var invoiceLine = new InvoiceLine
        {
            Value = 100.00M,
            FundCode = "34ERTY6",
            MainAccount = "SOS273",
            SchemeCode = "DR5678",
            MarketingYear = 2023,
            Description = "P1 to P2 Transfer - 100% EU / Leader - M19 FA6a / RDD - LEADER"
        };

        Assert.Equal(100.00M, invoiceLine.Value);
        Assert.Equal("34ERTY6", invoiceLine.FundCode);
        Assert.Equal("SOS273", invoiceLine.MainAccount);
        Assert.Equal("DR5678", invoiceLine.SchemeCode);
        Assert.Equal(2023, invoiceLine.MarketingYear);
        Assert.Equal("RP00", invoiceLine.DeliveryBody);
        Assert.Equal("P1 to P2 Transfer - 100% EU / Leader - M19 FA6a / RDD - LEADER", invoiceLine.Description);
    }
}
