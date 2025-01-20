# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ChunckerService.csproj", "microservices/ChunckerService/"]
RUN dotnet restore "microservices/ChunckerService/ChunckerService.csproj"

WORKDIR "/src/microservices/ChunckerService"
COPY . .
RUN dotnet build "ChunckerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChunckerService.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir -p /root/.aws
COPY .aws/config /root/.aws
COPY .aws/credentials /root/.aws
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8078
EXPOSE 8078
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChunckerService.dll"]
