# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /project
EXPOSE 5056
EXPOSE 5057

# Copy project files
COPY ["GameApi/GameApi.csproj", "GameApi/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["YourSolution.sln", "./"]

# Restore dependencies
RUN dotnet restore "GameApi/GameApi.csproj"

# Copy full source
COPY . .

# Build and publish
WORKDIR "/src/GameApi"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GameApi.dll"]