# bouchon-universel

Application web ASP.NET Core permettant de créer et gérer des « bouchons » HTTP (services simulés / mocks). Elle expose une API pour définir des services, des environnements et leurs réponses, avec un stockage SQLite via Entity Framework Core et une documentation Swagger.

## Utilisation

Prérequis : SDK .NET 10.

```bash
dotnet build
dotnet run --project BouchonUniversel
```

L'interface Swagger de l'API est disponible sur `/swagger`. Un `Dockerfile` est également fourni pour une exécution en conteneur.

## Tests

```bash
dotnet test
```

Les tests unitaires (xUnit) couvrent la logique métier de bouchonnage : ajustement des dates
(`MockRealTime`) et chargement en cache de la configuration des patterns (`PatternDateFormatProvider`).

## Techno

C# / ASP.NET Core (.NET 10), Entity Framework Core 10 (SQLite), Swagger (Swashbuckle), xUnit.
