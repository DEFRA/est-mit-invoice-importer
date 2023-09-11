ARG PARENT_VERSION=1.5.0-dotnet6.0

# Development
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development

ARG PARENT_VERSION

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

RUN mkdir -p /home/dotnet/EST.MIT.InvoiceImporter.Function/ /home/dotnet/EST.MIT.InvoiceImporter.Function.Test/

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/*.csproj ./EST.MIT.InvoiceImporter.Function/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function/EST.MIT.InvoiceImporter.Function.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Test/*.csproj ./EST.MIT.InvoiceImporter.Function.Test/
RUN dotnet restore ./EST.MIT.InvoiceImporter.Function.Test/EST.MIT.InvoiceImporter.Function.Test.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function/ ./EST.MIT.InvoiceImporter.Function/
COPY --chown=dotnet:dotnet ./EST.MIT.InvoiceImporter.Function.Test/ ./EST.MIT.InvoiceImporter.Function.Test/

RUN dotnet publish ./EST.MIT.InvoiceImporter.Function/ -c Release -o /home/dotnet/out

ARG PORT=3000
ENV PORT ${PORT}
EXPOSE ${PORT}

CMD cd ./EST.MIT.InvoiceImporter.Function && func start --port ${PORT}

# Production
FROM defradigital/dotnetcore:$PARENT_VERSION AS production

ARG PARENT_VERSION
ARG PARENT_REGISTRY

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

ARG PORT=3000
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

COPY --from=development /home/dotnet/out/ ./

ENTRYPOINT ["dotnet", "EST.MIT.InvoiceImporter.Function.dll"]
