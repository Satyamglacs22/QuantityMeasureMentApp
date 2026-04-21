# Use the official .NET 10 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy the solution and project files first to cache layers
COPY *.sln ./
COPY QuantityMeasurementWebApi/*.csproj ./QuantityMeasurementWebApi/
COPY QuantityMeasurementAppBusiness/*.csproj ./QuantityMeasurementAppBusiness/
COPY QuantityMeasurementAppModels/*.csproj ./QuantityMeasurementAppModels/
COPY QuantityMeasurementAppRepositories/*.csproj ./QuantityMeasurementAppRepositories/
COPY QuantityMeasurementAppServices/*.csproj ./QuantityMeasurementAppServices/
COPY QuantityMeasurementApp.Tests/*.csproj ./QuantityMeasurementApp.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all the remaining source code
COPY . ./

# Build and publish the Web API project
RUN dotnet publish QuantityMeasurementWebApi/QuantityMeasurementWebApi.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Render exposes the port dynamically on the PORT environment variable
ENV ASPNETCORE_URLS=http://+:${PORT:-5000}

ENTRYPOINT ["dotnet", "QuantityMeasurementWebApi.dll"]
