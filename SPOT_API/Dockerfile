#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SPOT_API/CarTradePro.API.csproj", "SPOT_API/"]
COPY ["Model/Model.csproj", "Model/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "SPOT_API/CarTradePro.API.csproj"
COPY . .
WORKDIR "/src/SPOT_API"
RUN dotnet build "CarTradePro.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarTradePro.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarTradePro.API.dll"]