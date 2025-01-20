# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ChunkerService.csproj", "microservices/ChunkerService/"]
RUN dotnet restore "microservices/ChunkerService/ChunkerService.csproj"

WORKDIR "/src/microservices/ChunkerService"
COPY . .
RUN dotnet build "ChunkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChunkerService.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir -p /root/.aws
COPY .aws/config /root/.aws
COPY .aws/credentials /root/.aws
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8078
EXPOSE 8078
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChunkerService.dll"]
