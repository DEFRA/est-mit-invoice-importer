﻿using EST.MIT.InvoiceImporter.Function.Builders;
using FluentAssertions;

namespace EST.MIT.InvoiceImporter.Function.Test.Builders;

public class NotificationBuilderTests
{
    [Fact]
    public void Build_WhenIdIsNull_ThrowsInvalidOperationException()
    {
        var builder = new NotificationBuilder();

        Action action = () => builder.Build();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Id cannot be null or empty.");
    }

    [Fact]
    public void Build_WhenSchemeIsNull_ThrowsInvalidOperationException()
    {
        var builder = new NotificationBuilder()
            .WithId("123");

        Action action = () => builder.Build();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Scheme cannot be null or empty.");
    }

    [Fact]
    public void Build_WhenActionIsNull_ThrowsInvalidOperationException()
    {
        var builder = new NotificationBuilder()
            .WithId("123")
            .WithScheme("scheme");

        Action action = () => builder.Build();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Action cannot be null.");
    }
}