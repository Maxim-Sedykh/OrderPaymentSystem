FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderPaymentSystem.Api/OrderPaymentSystem.Api.csproj", "OrderPaymentSystem.Api/"]
COPY ["OrderPaymentSystem.DAL/OrderPaymentSystem.DAL.csproj", "OrderPaymentSystem.DAL/"]
COPY ["OrderPaymentSystem.Domain/OrderPaymentSystem.Domain.csproj", "OrderPaymentSystem.Domain/"]
COPY ["OrderPaymentSystem.Application/OrderPaymentSystem.Application.csproj", "OrderPaymentSystem.Application/"]
COPY ["OrderPaymentSystem.Producer/OrderPaymentSystem.Producer.csproj", "OrderPaymentSystem.Producer/"]
COPY ["OrderPaymentSystem.Consumer/OrderPaymentSystem.Consumer.csproj", "OrderPaymentSystem.Consumer/"]
RUN dotnet restore "./OrderPaymentSystem.Api/./OrderPaymentSystem.Api.csproj"
COPY . .
WORKDIR "/src/OrderPaymentSystem.Api"
RUN dotnet build "./OrderPaymentSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderPaymentSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderPaymentSystem.Api.dll"]