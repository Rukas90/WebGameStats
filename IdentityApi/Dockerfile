# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /project
EXPOSE 5066
EXPOSE 5067

# Copy project files
COPY ["IdentityApi/IdentityApi.csproj", "IdentityApi/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["YourSolution.sln", "./"]

# Restore dependencies
RUN dotnet restore "IdentityApi/IdentityApi.csproj"

# Copy full source
COPY . .

# Build and publish
WORKDIR "/src/IdentityApi"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityApi.dll"]