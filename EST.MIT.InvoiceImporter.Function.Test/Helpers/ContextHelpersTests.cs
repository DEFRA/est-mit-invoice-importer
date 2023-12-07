using EST.MIT.InvoiceImporter.Function.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace EST.MIT.InvoiceImporter.Function.Test.Helpers;

public class ContextHelpersTests
{
    [Fact]
    public void GetBaseURI_Returns_String()
    {

        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost");

        context.GetBaseURI().Should().Be("http://localhost");
    }
}