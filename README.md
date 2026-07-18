# bouchon-universel

Application web ASP.NET Core permettant de créer et gérer des « bouchons » HTTP (services simulés / mocks). Elle expose une API pour définir des services, des environnements et leurs réponses, avec un stockage SQLite via Entity Framework Core et une documentation Swagger.

## Utilisation

Prérequis : SDK .NET 5.

```bash
dotnet build
dotnet run --project BouchonUniversel
```

L'interface Swagger de l'API est disponible sur `/swagger`. Un `Dockerfile` est également fourni pour une exécution en conteneur.

## Techno

C# / ASP.NET Core (.NET 5), Entity Framework Core (SQLite), Swagger.
