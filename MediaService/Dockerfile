# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MediaService.csproj", "microservices/MediaService/"]
RUN dotnet restore "microservices/MediaService/MediaService.csproj"

WORKDIR "/src/microservices/MediaService"
COPY . .
RUN dotnet build "MediaService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaService.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir -p /root/.aws
COPY .aws/config /root/.aws
COPY .aws/credentials /root/.aws
WORKDIR /app
EXPOSE 8078
EXPOSE 8077
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.dll"]
