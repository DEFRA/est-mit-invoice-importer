ARG PARENT_VERSION=1.5.0-dotnet6.0

# /home/site/wwwroot

# Development
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS development

ARG PARENT_VERSION

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

RUN mkdir -p /home/dotnet/EST.MIT.InvoiceImporter.Function/ /home/dotnet/EST.MIT.InvoiceImporter.Function.Test/ 

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/*.csproj ./EST.MIT.InvoiceImporter.Function/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function/EST.MIT.InvoiceImporter.Function.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Test/*.csproj ./EST.MIT.InvoiceImporter.Function.Test/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function.Test/EST.MIT.InvoiceImporter.Function.Test.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/ ./EST.MIT.InvoiceImporter.Function/
COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Test/ ./EST.MIT.InvoiceImporter.Function.Test/

COPY ./EST.MIT.InvoiceImporter.Function /src
RUN cd /src && \
    mkdir -p /tmp/site/wwwroot && \
    dotnet publish *.csproj --output /tmp/site/wwwroot

ARG PORT=3000
ENV PORT ${PORT}
EXPOSE ${PORT}

FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ENV ASPNETCORE_URLS=http://+:3000
ENV FUNCTIONS_WORKER_RUNTIME=dotnet
COPY --from=development ["/tmp/site/wwwroot", "/home/site/wwwroot"]

# Production
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS production

ARG PARENT_VERSION
ARG PARENT_REGISTRY

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

ARG PORT=3000
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

COPY ./EST.MIT.InvoiceImporter.Function /src
RUN cd /src && \
    mkdir -p /tmp/site/wwwroot && \
    dotnet publish *.csproj -c Release -o /tmp/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ENV ASPNETCORE_URLS=http://+:3000
ENV FUNCTIONS_WORKER_RUNTIME=dotnet
COPY --from=production ["/tmp/site/wwwroot", "/home/site/wwwroot"]
