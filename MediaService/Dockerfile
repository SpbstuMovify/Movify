# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/MediaService/MediaService.csproj MediaService/
RUN dotnet restore "MediaService/MediaService.csproj"

WORKDIR /src/MediaService
COPY src/MediaService/ .
RUN dotnet build "MediaService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 8078
EXPOSE 8077
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.dll"]
