﻿FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PAiPWebPackageManager/PAiPWebPackageManager.csproj", "PAiPWebPackageManager/"]
RUN dotnet restore "PAiPWebPackageManager/PAiPWebPackageManager.csproj"
COPY . .
WORKDIR "/src/PAiPWebPackageManager"
RUN dotnet build "PAiPWebPackageManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PAiPWebPackageManager.csproj" -c Release -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/PAiPWebPackageManager ./pwpm
ENTRYPOINT ["./PAiPWebPackageManager"]
