# bouchon-universel

Application web ASP.NET Core permettant de créer et gérer des « bouchons » HTTP (services simulés / mocks). Elle expose une API pour définir des services, des environnements et leurs réponses, avec un stockage SQLite via Entity Framework Core et une documentation Swagger.

## Utilisation

Prérequis : SDK .NET 10.

```bash
dotnet build
dotnet run --project BouchonUniversel
```

L'interface Swagger de l'API est disponible sur `/swagger`. Un `Dockerfile` est également fourni pour une exécution en conteneur.

Les verbes **GET, POST, PUT, PATCH et DELETE** sont pris en charge par le bouchon. En cas d'erreur,
l'API renvoie un code HTTP sémantique (404 si la clé ou l'environnement est introuvable, 500 sinon).
Le contenu des réponses mockées peut être édité depuis la page de détail d'un service.

## Base de données

Le schéma SQLite est géré par les **migrations Entity Framework Core**. Les migrations en attente
sont appliquées automatiquement au démarrage (`Database.Migrate()`), et les paramètres initiaux sont
amorcés depuis `appsettings.json`.

Pour faire évoluer le schéma après modification du modèle :

```bash
dotnet ef migrations add <NomDeLaMigration> --project BouchonUniversel
```

La migration est appliquée au prochain lancement de l'application (ou via `dotnet ef database update`).

## Ingénierie du chaos

Chaque service peut simuler une **latence** (en millisecondes) et l'**injection d'erreurs**
(probabilité 0-100 % + code HTTP renvoyé), configurables depuis les écrans de création/édition d'un service.

## Import / export de jeux de mocks

- **Export** : « Télécharger les fichiers » produit une archive `tar.gz` de toutes les réponses mockées.
- **Import** : « Importer des fichiers » téléverse une archive `tar.gz` et l'extrait dans le répertoire des bouchons
  (extraction protégée contre la traversée de répertoire).

## Authentification de l'administration

Les écrans d'administration peuvent être protégés par une authentification par cookie. Elle est **activée
uniquement** si un identifiant et un mot de passe sont configurés dans la section `Admin` :

```bash
# via variables d'environnement (recommandé plutôt que appsettings.json)
Admin__Username=admin
Admin__Password=<mot-de-passe>
```

L'API de bouchonnage (`/api/bouchon/*`) et la page d'installation restent toujours accessibles sans authentification.

## Tests

```bash
dotnet test
```

Les tests unitaires (xUnit) couvrent la logique métier de bouchonnage : ajustement des dates
(`MockRealTime`) et chargement en cache de la configuration des patterns (`PatternDateFormatProvider`).

## Techno

C# / ASP.NET Core (.NET 10), Entity Framework Core 10 (SQLite), Swagger (Swashbuckle), xUnit.
