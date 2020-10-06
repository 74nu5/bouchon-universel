FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY BouchonUniversel/*.csproj ./BouchonUniversel/
COPY BouchonUniversel.Utils/*.csproj ./BouchonUniversel.Utils/
COPY BouchonUniversel.ApiTest/*.csproj ./BouchonUniversel.ApiTest/
RUN dotnet restore

# copy and build everything else
COPY BouchonUniversel/. ./BouchonUniversel/
COPY BouchonUniversel.Utils/. ./BouchonUniversel.Utils/
COPY BouchonUniversel.ApiTest/. ./BouchonUniversel.ApiTest/
RUN dotnet build

FROM build AS publish
WORKDIR /app/BouchonUniversel
RUN dotnet publish -o out

FROM microsoft/dotnet:2.0-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/BouchonUniversel/out ./
ENTRYPOINT ["dotnet", "BouchonUniversel.dll"]
