FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OrderPaymentSystem.slnx", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/Infrastructure/OrderPaymentSystem.DAL/OrderPaymentSystem.DAL.csproj", "src/Infrastructure/OrderPaymentSystem.DAL/"]
COPY ["src/Core/OrderPaymentSystem.Domain/OrderPaymentSystem.Domain.csproj", "src/Core/OrderPaymentSystem.Domain/"]
COPY ["src/Core/OrderPaymentSystem.Application/OrderPaymentSystem.Application.csproj", "src/Core/OrderPaymentSystem.Application/"]
COPY ["src/Core/OrderPaymentSystem.Shared/OrderPaymentSystem.Shared.csproj", "src/Core/OrderPaymentSystem.Shared/"]
COPY ["src/Presentation/OrderPaymentSystem.Api/OrderPaymentSystem.Api.csproj", "src/Presentation/OrderPaymentSystem.Api/"]

RUN dotnet restore "src/Presentation/OrderPaymentSystem.Api/OrderPaymentSystem.Api.csproj"

COPY . .

WORKDIR "/src/src/Presentation/OrderPaymentSystem.Api"
RUN dotnet build "OrderPaymentSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderPaymentSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderPaymentSystem.Api.dll"]