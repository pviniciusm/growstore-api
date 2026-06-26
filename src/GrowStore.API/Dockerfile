FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/GrowStore.API/GrowStore.API.csproj", "src/GrowStore.API/"]
COPY ["src/GrowStore.Application/GrowStore.Application.csproj", "src/GrowStore.Application/"]
COPY ["src/GrowStore.Domain/GrowStore.Domain.csproj", "src/GrowStore.Domain/"]
COPY ["src/GrowStore.Infrastructure/GrowStore.Infrastructure.csproj", "src/GrowStore.Infrastructure/"]

RUN dotnet restore "src/GrowStore.API/GrowStore.API.csproj"

COPY . .
WORKDIR "/src/src/GrowStore.API"
RUN dotnet build "GrowStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrowStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrowStore.API.dll"]