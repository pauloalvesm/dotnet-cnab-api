FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CNAB.WebAPI/CNAB.WebAPI.csproj", "src/CNAB.WebAPI/"]
COPY ["src/CNAB.Infra.IoC/CNAB.Infra.IoC.csproj", "src/CNAB.Infra.IoC/"]
COPY ["src/CNAB.Application/CNAB.Application.csproj", "src/CNAB.Application/"]
COPY ["src/CNAB.Domain/CNAB.Domain.csproj", "src/CNAB.Domain/"]
COPY ["src/CNAB.Infra.Data/CNAB.Infra.Data.csproj", "src/CNAB.Infra.Data/"]
RUN dotnet restore "src/CNAB.WebAPI/CNAB.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/CNAB.WebAPI"
RUN dotnet publish "CNAB.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CNAB.WebAPI.dll"]