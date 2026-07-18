# bouchon-universel

Application web ASP.NET Core permettant de créer et gérer des « bouchons » HTTP (services simulés / mocks). Elle expose une API pour définir des services, des environnements et leurs réponses, avec un stockage SQLite via Entity Framework Core et une documentation Swagger.

## Démarrage rapide

Choisissez l'option qui vous convient — l'application est ensuite disponible sur **http://localhost:8080**.

**Docker (rien à installer d'autre)**

```bash
docker run -p 8080:8080 -v bouchon-data:/app/data ghcr.io/74nu5/bouchon-universel:latest
# ou, depuis le dépôt cloné :
docker compose up
```

**Binaire autonome (sans .NET installé)**

```bash
# Linux / macOS
curl -fsSL https://raw.githubusercontent.com/74nu5/bouchon-universel/master/install.sh | sh
```

```powershell
# Windows (PowerShell)
irm https://raw.githubusercontent.com/74nu5/bouchon-universel/master/install.ps1 | iex
```

Les binaires par système sont aussi téléchargeables depuis les [Releases GitHub](https://github.com/74nu5/bouchon-universel/releases).

## Documentation

📖 **Site en ligne : https://ambitious-river-06e7d3903.7.azurestaticapps.net**

Un site de documentation **Blazor WebAssembly** (statique) se trouve dans `BouchonUniversel.Docs`
(Introduction, Démarrage rapide, Installation, Configuration, Utilisation, Fonctionnalités, Référence API,
FAQ). Il est déployé sur **Azure Static Web Apps** via le workflow
`.github/workflows/azure-static-web-apps.yml` (secret `AZURE_STATIC_WEB_APPS_API_TOKEN`).

Pour le prévisualiser localement :

```bash
dotnet run --project BouchonUniversel.Docs
```

## Développement

Prérequis : SDK .NET 10.

```bash
dotnet build
dotnet run --project BouchonUniversel
```

L'interface Swagger de l'API est disponible sur `/swagger`. Un `Dockerfile` est également fourni pour une exécution en conteneur.

Les verbes **GET, POST, PUT, PATCH, DELETE et HEAD** sont pris en charge par le bouchon, avec **CORS**
permissif (appel depuis n'importe quelle origine, préflight OPTIONS géré). En cas d'erreur, l'API renvoie
un code HTTP sémantique (404 si la clé ou l'environnement est introuvable, 500 sinon). Le contenu des
réponses mockées peut être édité depuis la page de détail d'un service.

Les réponses rejouées peuvent être **templatées** (activable par service) : jetons `{{route}}`,
`{{query.NOM}}`, `{{header.NOM}}`, `{{guid}}`, `{{now}}` / `{{now:FORMAT}}`. L'ingénierie du chaos couvre
aussi la **coupure de connexion** et la **réponse tronquée**.

Une sonde de santé est exposée sur `/health` (vérifie aussi l'accès à la base). Toutes les réponses
portent des en-têtes de sécurité (CSP, `X-Content-Type-Options`, `X-Frame-Options`, HSTS hors développement).
Une sauvegarde de la base peut être téléchargée depuis « Sauvegarder la base » (copie SQLite cohérente).
Les journaux sont **structurés** (Serilog) et les modifications d'administration sont **auditées**.

## Conteneur

```bash
# Lancement complet avec volume persistant (base + fichiers de bouchons)
docker compose up --build            # écoute sur http://localhost:8080

# Image multi-architecture
docker buildx build --platform linux/amd64,linux/arm64 -t bouchon-universel:latest .
```

En conteneur, l'application écoute selon `ASPNETCORE_URLS` (ex. `http://+:8080`) ; en local sans cette
variable, elle écoute sur `https://*:5555`.

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
uniquement** si un identifiant et un mot de passe sont configurés dans la section `Admin`.

Le mot de passe doit de préférence être fourni **haché** (PBKDF2 salé, format `PasswordHasher` ASP.NET Core).
Générer le hash puis le placer dans `Admin:PasswordHash` :

```bash
dotnet run --project BouchonUniversel -- hash-password "<mot-de-passe>"
# → copier la valeur affichée dans Admin:PasswordHash
```

```bash
# via variables d'environnement (recommandé plutôt que appsettings.json)
Admin__Username=admin
Admin__PasswordHash=<hash-généré>
```

Un mot de passe en clair (`Admin:Password`) reste accepté en dépannage/développement (comparaison à temps
constant), mais `Admin:PasswordHash` est le chemin recommandé.

Un **compte lecteur** facultatif (`Admin:ViewerUsername` / `Admin:ViewerPasswordHash`) donne un accès en
**lecture seule** : les modifications (création/édition/suppression, import) exigent le rôle administrateur.

L'API de bouchonnage (`/api/bouchon/*`) et la page d'installation restent toujours accessibles sans authentification.

## Tests

```bash
dotnet test
```

- **Tests unitaires** (xUnit) : logique métier (ajustement des dates `MockRealTime`, cache des patterns,
  chaos, mapping d'erreurs, sécurité des chemins, hachage du mot de passe, upsert des paramètres).
- **Tests d'intégration bout-en-bout** (`WebApplicationFactory`, base SQLite et fichiers isolés) : pipeline
  HTTP complet — healthcheck, codes d'erreur sémantiques sur tous les verbes, injection de chaos,
  redirection d'authentification et API restée ouverte.

## Interface d'administration

L'interface est en **Bootstrap 5** avec un **thème clair/sombre** commutable (persisté, respecte les
préférences système). Les listes disposent d'un **filtre de recherche** instantané, les services peuvent être
**dupliqués**, et l'éditeur de réponses mockées offre une **coloration syntaxique** (CodeMirror) avec
validation JSON/XML.

## Techno

C# / ASP.NET Core (.NET 10), Entity Framework Core 10 (SQLite), Serilog, Swagger (Swashbuckle),
Bootstrap 5 + CodeMirror, xUnit.
