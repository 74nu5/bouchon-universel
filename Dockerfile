# syntax=docker/dockerfile:1

# --- Étape de build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copie des csproj et restauration en couche distincte (cache Docker).
COPY *.sln .
COPY BouchonUniversel/*.csproj ./BouchonUniversel/
COPY BouchonUniversel.Utils/*.csproj ./BouchonUniversel.Utils/
COPY BouchonUniversel.ApiTest/*.csproj ./BouchonUniversel.ApiTest/
COPY BouchonUniversel.Tests/*.csproj ./BouchonUniversel.Tests/
RUN dotnet restore BouchonUniversel/BouchonUniversel.csproj

# Copie du reste et publication (framework-dependent, sur l'image aspnet).
COPY . .
RUN dotnet publish BouchonUniversel/BouchonUniversel.csproj \
    -c Release -o /app/publish \
    --no-self-contained -p:PublishSingleFile=false -p:PublishReadyToRun=false

# --- Étape d'exécution ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configuration par défaut : écoute HTTP sur 8080, base et fichiers dans un volume persistant.
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__BDDConnection=/app/data/BouchonUniversel.db \
    Bouchon__CheminFichiers=/app/data/files \
    Bouchon__DefautActivation=true \
    Bouchon__UrlService=""

RUN mkdir -p /app/data/files
VOLUME /app/data
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "BouchonUniversel.dll"]
