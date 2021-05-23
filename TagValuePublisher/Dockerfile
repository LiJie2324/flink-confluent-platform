FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TagValuePublisher.csproj", "./"]
RUN dotnet restore "TagValuePublisher.csproj"
COPY . .
WORKDIR "/src/TagValuePublisher"
RUN dotnet build "TagValuePublisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TagValuePublisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TagValuePublisher.dll"]
