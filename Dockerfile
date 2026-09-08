FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY ["ClinicFlow.Api/ClinicFlow.Api.csproj", "ClinicFlow.Api/"]
COPY ["ClinicFlow.Application/ClinicFlow.Application.csproj", "ClinicFlow.Application/"]
COPY ["ClinicFlow.Domain/ClinicFlow.Domain.csproj", "ClinicFlow.Domain/"]
COPY ["ClinicFlow.Infrastructure/ClinicFlow.Infrastructure.csproj", "ClinicFlow.Infrastructure/"]

RUN dotnet restore "ClinicFlow.Api/ClinicFlow.Api.csproj"

COPY . .

WORKDIR "/app/ClinicFlow.Api"

RUN dotnet publish "ClinicFlow.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicFlow.Api.dll"]