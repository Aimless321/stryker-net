FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Stryker.CLI/Stryker.CLI/Stryker.CLI.csproj", "Stryker.CLI/Stryker.CLI/"]
COPY ["Stryker.Core/Stryker.Core/Stryker.Core.csproj", "Stryker.Core/Stryker.Core/"]
COPY ["Stryker.Abstractions/Stryker.Abstractions.csproj", "Stryker.Abstractions/"]
COPY ["Stryker.Utilities/Stryker.Utilities.csproj", "Stryker.Utilities/"]
COPY ["Stryker.DataCollector/Stryker.DataCollector/Stryker.DataCollector.csproj", "Stryker.DataCollector/Stryker.DataCollector/"]
COPY ["Stryker.Options/Stryker.Configuration.csproj", "Stryker.Options/"]
COPY ["Stryker.RegexMutators/Stryker.RegexMutators/Stryker.RegexMutators.csproj", "Stryker.RegexMutators/Stryker.RegexMutators/"]
RUN dotnet restore "Stryker.CLI/Stryker.CLI/Stryker.CLI.csproj"
COPY . .
WORKDIR "/src/Stryker.CLI/Stryker.CLI"
RUN dotnet build "Stryker.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Stryker.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stryker.CLI.dll", "start-server"]
