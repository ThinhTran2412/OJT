# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.config first (for local package source)
COPY ["NuGet.config", "./"]

# Copy local nuget packages
COPY ["nuget/", "./nuget/"]

# Copy csproj files first for better layer caching
COPY ["Laboratory_Service.API/Laboratory_Service.API.csproj", "Laboratory_Service.API/"]
COPY ["Laboratory_Service.Application/Laboratory_Service.Application.csproj", "Laboratory_Service.Application/"]
COPY ["Laboratory_Service.Domain/Laboratory_Service.Domain.csproj", "Laboratory_Service.Domain/"]
COPY ["Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj", "Laboratory_Service.Infrastructure/"]
COPY ["ShareLibrarySolution/SharedLibrary.csproj", "ShareLibrarySolution/"]

# Copy proto files from other services (required for gRPC clients)
# Create directory structure to match the reference paths in csproj files
# Path 1: ../../IAM_Service/IAM_Service.API/Protos/user_service.proto
RUN mkdir -p ../IAM_Service/IAM_Service.API/Protos
COPY ["ExternalProtos/IAM_Service/IAM_Service.API/Protos/user_service.proto", "../IAM_Service/IAM_Service.API/Protos/"]

# Path 2: ../../Simulator_Service/Simulator.API/Protos/simulator_query.proto
RUN mkdir -p ../Simulator_Service/Simulator.API/Protos
COPY ["ExternalProtos/Simulator_Service/Simulator.API/Protos/simulator_query.proto", "../Simulator_Service/Simulator.API/Protos/"]

# Restore dependencies (NuGet.config will use local nuget source)
RUN dotnet restore "./Laboratory_Service.API/Laboratory_Service.API.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/Laboratory_Service.API"
RUN dotnet build "./Laboratory_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Laboratory_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy published files
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "Laboratory_Service.API.dll"]

