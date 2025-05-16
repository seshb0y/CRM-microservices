FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Скопировать .csproj и зависимости
COPY Crm.Grpc.Contracts/ Crm.Grpc.Contracts/
COPY ClientService/ ClientService/

# Восстановить зависимости
RUN dotnet restore ./ClientService/ClientService/ClientService.csproj

# Сборка
WORKDIR /src/ClientService/ClientService
RUN dotnet publish ClientService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClientService.dll"]
