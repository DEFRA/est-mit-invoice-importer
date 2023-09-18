using EST.MIT.InvoiceImporter.Function.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class PaymentRequestsBatchTests
{
    [Fact]
    public void TestPaymentRequestsBatchProperties()
    {
        var expectedTime = DateTimeOffset.Now;

        var paymentRequestsBatch = new PaymentRequestsBatch
        {
            Id = "5c3e1c3e-21d6-4085-8c1a-c564ce9d9d39",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "7ff42cf1-2ae6-45f2-a9c0-3442e41e8d00",
            SchemeType = "bps",
            PaymentType = "DOM",
            Status = "awaiting",
            CreatedBy = "Test User",
            Created = expectedTime,
            Updated = expectedTime,
            UpdatedBy = "Test User"
        };

        Assert.Equal("5c3e1c3e-21d6-4085-8c1a-c564ce9d9d39", paymentRequestsBatch.Id);
        Assert.Equal("AP", paymentRequestsBatch.AccountType);
        Assert.Equal("Test Org", paymentRequestsBatch.Organisation);
        Assert.Equal("7ff42cf1-2ae6-45f2-a9c0-3442e41e8d00", paymentRequestsBatch.Reference);
        Assert.Equal("bps", paymentRequestsBatch.SchemeType);
        Assert.Equal("DOM", paymentRequestsBatch.PaymentType);
        Assert.Equal("awaiting", paymentRequestsBatch.Status);
        Assert.Equal("Test User", paymentRequestsBatch.CreatedBy);
        Assert.Equal("Test User", paymentRequestsBatch.UpdatedBy);
        Assert.Equal(expectedTime, paymentRequestsBatch.Created);
        Assert.Equal(expectedTime, paymentRequestsBatch.Updated);
    }

}
