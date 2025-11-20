# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.config first (for local package source)
COPY ["NuGet.config", "./"]

# Copy local nuget packages
COPY ["nuget/", "./nuget/"]

# Copy csproj files for better layer caching
COPY ["IAM_Service.API/IAM_Service.API.csproj", "IAM_Service.API/"]
COPY ["IAM_Service.Application/IAM_Service.Application.csproj", "IAM_Service.Application/"]
COPY ["IAM_Service.Domain/IAM_Service.Domain.csproj", "IAM_Service.Domain/"]
COPY ["IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj", "IAM_Service.Infrastructure/"]
COPY ["ShareLibrarySolution/SharedLibrary/SharedLibrary.csproj", "ShareLibrarySolution/SharedLibrary/"]

# Restore dependencies (NuGet.config will use local nuget source)
RUN dotnet restore "./IAM_Service.API/IAM_Service.API.csproj"
COPY . .
WORKDIR "/src/IAM_Service.API"
RUN dotnet build "./IAM_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IAM_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IAM_Service.API.dll"]