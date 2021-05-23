FROM warrenhuang/dotnetsdk:5.0.203 AS base
WORKDIR /app

FROM warrenhuang/dotnetsdk:SDK_3.1.409_AVRO_1.10.2 AS prebuild
WORKDIR /src
COPY avro .
RUN for schema_file in ./*.avsc; do \
      avrogen -s $schema_file /src/codegen; \
    done

FROM warrenhuang/dotnetsdk:5.0.203 AS build
WORKDIR /src
COPY ["TagValuePublisher.csproj", "./"]
RUN dotnet restore "TagValuePublisher.csproj"
COPY . .
COPY --from=prebuild /src/codegen /src/codegen
RUN dotnet build "TagValuePublisher.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "TagValuePublisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TagValuePublisher.dll"]
