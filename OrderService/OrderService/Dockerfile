FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# КОРРЕКТНО скопировать .csproj и зависимости
COPY Crm.Grpc.Contracts/ /src/Crm.Grpc.Contracts/
COPY OrderService/ OrderService/

# Восстановить зависимости
RUN dotnet restore ./OrderService/OrderService/OrderService.csproj

# Сборка
WORKDIR /src/OrderService/OrderService
RUN dotnet publish OrderService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
