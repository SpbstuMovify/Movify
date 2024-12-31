# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/microservices/AuthMicroservice/AuthMicroservice.csproj", "microservices/AuthMicroservice/"]
COPY src/middlewares middlewares/
RUN dotnet restore "microservices/AuthMicroservice/AuthMicroservice.csproj"

WORKDIR "/src/microservices/AuthMicroservice"
COPY src/microservices/AuthMicroservice .
RUN dotnet build "AuthMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthMicroservice.dll"]