# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ElasticTraining.csproj", "./"]
RUN dotnet restore "ElasticTraining.csproj"
COPY . .
RUN dotnet publish "ElasticTraining.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Configure the application to listen on port 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ElasticTraining.dll"]
