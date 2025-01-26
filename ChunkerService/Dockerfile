FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ChunkerService/ChunkerService.csproj ChunkerService/
RUN dotnet restore "ChunkerService/ChunkerService.csproj"

WORKDIR /src/ChunkerService
COPY src/ChunkerService/ .
RUN dotnet build "ChunkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChunkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 8078
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChunkerService.dll"]
