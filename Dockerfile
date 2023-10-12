# Development
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS development

RUN mkdir -p /home/dotnet/EST.MIT.InvoiceImporter.Function/ /home/dotnet/EST.MIT.InvoiceImporter.Function.Test/ 

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/*.csproj ./EST.MIT.InvoiceImporter.Function/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function/EST.MIT.InvoiceImporter.Function.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Test/*.csproj ./EST.MIT.InvoiceImporter.Function.Test/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function.Test/EST.MIT.InvoiceImporter.Function.Test.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/ ./EST.MIT.InvoiceImporter.Function/
COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Test/ ./EST.MIT.InvoiceImporter.Function.Test/

COPY ./EST.MIT.InvoiceImporter.Function /src
RUN cd /src && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

ARG PORT=3000
ENV PORT ${PORT}
EXPOSE ${PORT}

FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ENV ASPNETCORE_URLS=http://*:${PORT}
ENV FUNCTIONS_WORKER_RUNTIME=dotnet
COPY --from=development ["/home/site/wwwroot", "/home/site/wwwroot"]

# Production
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS production

ARG PORT=3000
ENV PORT ${PORT}

COPY ./EST.MIT.InvoiceImporter.Function /src
RUN cd /src && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ENV ASPNETCORE_URLS=http://*:${PORT}
ENV FUNCTIONS_WORKER_RUNTIME=dotnet
COPY --from=production ["/home/site/wwwroot", "/home/site/wwwroot"]
