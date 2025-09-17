FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/", "src/"]
RUN dotnet restore "src/maintenance/Bl.Mottu.Maintenance.Api/Bl.Mottu.Maintenance.Api.csproj"
COPY . .
WORKDIR "src/maintenance/Bl.Mottu.Maintenance.Api"
RUN dotnet build "Bl.Mottu.Maintenance.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bl.Mottu.Maintenance.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bl.Mottu.Maintenance.Api.dll"]