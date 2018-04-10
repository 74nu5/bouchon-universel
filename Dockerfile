FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY BouchonUniversel/*.csproj ./BouchonUniversel/
RUN dotnet restore

# copy and build everything else
COPY BouchonUniversel/. ./BouchonUniversel/
RUN dotnet build

FROM test AS publish
WORKDIR /app/BouchonUniversel
RUN dotnet publish -o out

FROM microsoft/dotnet:2.0-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/BouchonUniversel/out ./
ENTRYPOINT ["dotnet", "BouchonUniversel.dll"]
