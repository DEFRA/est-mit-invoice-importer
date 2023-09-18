using EST.MIT.InvoiceImporter.Function.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class PaymentRequestTests
{
    [Fact]
    public void TestPaymentRequestProperties()
    {
        var expectedTime = DateTime.Now;

        var paymentRequest = new PaymentRequest
        {
            PaymentRequestId = "7ff42cf1-2ae6-45f2-a9c0-3442e41e8d00",
            SourceSystem = "Manual",
            ClaimReferenceNumber = "1234567890",
            ClaimReference = "TestClaimReference",
            FRN = 107377217,
            MarketingYear = 2023,
            PaymentRequestNumber = 1,
            AgreementNumber = "ER456G",
            Currency = "GBP",
            Description = "Description",
            OriginalInvoiceNumber = "124169",
            OriginalSettlementDate = expectedTime,
            RecoveryDate = expectedTime,
            InvoiceCorrectionReference = "124202",
            DueDate = "2023-01-01",
            Value = 100.00M,
            SBI = 12345,
            Vendor = "9999999999"
        };

        Assert.Equal("7ff42cf1-2ae6-45f2-a9c0-3442e41e8d00", paymentRequest.PaymentRequestId);
        Assert.Equal("Manual", paymentRequest.SourceSystem);
        Assert.Equal("1234567890", paymentRequest.ClaimReferenceNumber);
        Assert.Equal("TestClaimReference", paymentRequest.ClaimReference);
        Assert.Equal(107377217, paymentRequest.FRN);
        Assert.Equal(2023, paymentRequest.MarketingYear);
        Assert.Equal(1, paymentRequest.PaymentRequestNumber);
        Assert.Equal("ER456G", paymentRequest.AgreementNumber);
        Assert.Equal("GBP", paymentRequest.Currency);
        Assert.Equal("Description", paymentRequest.Description);
        Assert.Equal("124169", paymentRequest.OriginalInvoiceNumber);
        Assert.Equal(expectedTime, paymentRequest.OriginalSettlementDate);
        Assert.Equal(expectedTime, paymentRequest.RecoveryDate);
        Assert.Equal("124202", paymentRequest.InvoiceCorrectionReference);
        Assert.Equal("2023-01-01", paymentRequest.DueDate);
        Assert.Equal(100.00M, paymentRequest.Value);
        Assert.Equal(12345, paymentRequest.SBI);
        Assert.Equal("9999999999", paymentRequest.Vendor);
    }
}
