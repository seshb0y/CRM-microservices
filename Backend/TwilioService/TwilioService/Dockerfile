FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
  
  # Скопировать .csproj и зависимости
COPY Crm.Grpc.Contracts/ Crm.Grpc.Contracts/
COPY TwilioService/ TwilioService/
  
  # Восстановить зависимости
RUN dotnet restore ./TwilioService/TwilioService/TwilioService.csproj
  
  # Сборка
WORKDIR /src/TwilioService/TwilioService
RUN dotnet publish TwilioService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TwilioService.dll"]
