# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/AuthService/AuthService.csproj AuthService/
RUN dotnet restore "AuthService/AuthService.csproj"

WORKDIR /src/AuthService
COPY src/AuthService/ .
RUN dotnet build "AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 8079
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]
