# development
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS development

RUN mkdir -p /home/dotnet/EST.MIT.InvoiceImporter.Function/ /home/dotnet/EST.MIT.InvoiceImporter.Function.Test/
COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Tests/*.csproj ./EST.MIT.InvoiceImporter.Function.Tests/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function.Tests/EST.MIT.InvoiceImporter.Function.Tests.csproj
COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/*.csproj ./EST.MIT.InvoiceImporter.Function/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function/EST.MIT.InvoiceImporter.Function.csproj