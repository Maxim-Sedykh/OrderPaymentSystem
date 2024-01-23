FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderPaymentSystem.Api/OrderPaymentSystem.Api.csproj", "OrderPaymentSystem.Api/"]
RUN dotnet restore "./OrderPaymentSystem.Api/./OrderPaymentSystem.Api.csproj"
COPY . .
WORKDIR "/src/OrderPaymentSystem.Api"
RUN dotnet build "./OrderPaymentSystem.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderPaymentSystem.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderPaymentSystem.Api.dll"]