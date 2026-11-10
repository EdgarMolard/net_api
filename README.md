# MonPremierProjet

API REST de gestion de tâches (`Todo`) réalisée avec ASP.NET Core et .NET 10.
Le projet utilise SQLite avec Entity Framework Core, des services asynchrones
et une authentification JWT pour protéger les opérations d'écriture.

> L'authentification actuelle est une démonstration pédagogique. L'endpoint
> `/tokendev` ne doit pas être conservé tel quel dans une application de
> production.

## Fonctionnalités

- consultation publique des Todo ;
- création, modification et suppression protégées par JWT ;
- persistance SQLite dans `app.db` ;
- migrations Entity Framework Core ;
- validation du titre (obligatoire, 100 caractères maximum) ;
- génération d'un token de développement ;
- tests unitaires du service Todo et de la génération JWT ;
- spécification OpenAPI en environnement Development.

## Prérequis

- .NET 10 SDK ;
- VS Code (facultatif, utile pour le fichier `.http`) ;
- l'extension **REST Client** de VS Code pour exécuter les requêtes `.http`.

Vérifier l'installation :

```bash
dotnet --version
```

## Installation

Depuis la racine du projet :

```bash
dotnet restore
```

La base SQLite est créée ou mise à jour à partir des migrations au besoin.
Elle est stockée dans [`app.db`](./app.db).

## Lancer l'API

### Avec `dotnet run`

Le profil HTTP de développement utilise le port `5155` :

```bash
dotnet run --launch-profile http
```

L'API est alors disponible à l'adresse :

```text
http://localhost:5155
```

Le profil HTTPS utilise :

```text
https://localhost:7162
```

### Depuis la DLL publiée

Construire puis lancer l'application :

```bash
dotnet build -c Release
ASPNETCORE_ENVIRONMENT=Development \
ASPNETCORE_URLS=http://localhost:5000 \
dotnet ./bin/Release/net10.0/MonPremierProjet.dll
```

Dans ce cas, l'API est disponible sur `http://localhost:5000`.

## Configuration JWT

En développement, la configuration se trouve dans
[`appsettings.Development.json`](./appsettings.Development.json).

La clé utilisée avec l'algorithme HS256 doit faire au moins 32 octets. Pour un
autre environnement, il est préférable de fournir la clé par variable
d'environnement :

```bash
Jwt__Key="une-cle-secrete-d-au-moins-32-caracteres" \
Jwt__Issuer="MonPremierProjet" \
dotnet ./bin/Release/net10.0/MonPremierProjet.dll
```

`Jwt__Key` correspond à la propriété de configuration `Jwt:Key`.

## Utiliser l'API

### Routes publiques

| Méthode | Route | Description |
|---|---|---|
| `GET` | `/todos` | Liste toutes les Todo |
| `GET` | `/todos/{id}` | Retourne une Todo |
| `GET` | `/tokendev` | Génère un token de développement |

### Routes protégées

Les routes suivantes nécessitent un en-tête `Authorization` :

| Méthode | Route | Description |
|---|---|---|
| `POST` | `/todos` | Crée une Todo |
| `PUT` | `/todos/{id}` | Modifie une Todo |
| `DELETE` | `/todos/{id}` | Supprime une Todo |

Format de l'en-tête :

```http
Authorization: Bearer <token>
```

### Exemple avec cURL

Récupérer un token :

```bash
curl http://localhost:5155/tokendev
```

Créer une Todo avec ce token :

```bash
curl -X POST http://localhost:5155/todos \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"title":"Apprendre ASP.NET Core","isCompleted":false}'
```

Une requête d'écriture sans token retourne `401 Unauthorized`.

## Tester avec VS Code

Ouvrir [`src/test/MonPremierProjet.http`](./src/test/MonPremierProjet.http).

1. Démarrer l'API.
2. Appeler `GET /tokendev`.
3. Copier la valeur `accessToken`.
4. Remplacer la valeur de `@accessToken`.
5. Exécuter les requêtes protégées.

Pour utiliser le profil HTTP de développement, modifier au besoin la première
ligne du fichier :

```http
@MonPremierProjet_HostAddress = http://localhost:5155
@accessToken = <token-obtenu-avec-tokendev>
```

## Tests automatisés

Lancer tous les tests de la solution :

```bash
dotnet test MonPremierProjet.slnx
```

Les tests couvrent notamment :

- l'attribution automatique d'un identifiant ;
- la suppression d'une Todo ;
- la recherche d'une Todo inexistante ;
- la signature, l'issuer et l'expiration du JWT.

## Migrations EF Core

Installer l'outil `dotnet-ef` si nécessaire :

```bash
dotnet tool install --global dotnet-ef
```

Créer une migration :

```bash
dotnet ef migrations add NomDeLaMigration
```

Appliquer les migrations :

```bash
dotnet ef database update
```

Les migrations existantes se trouvent dans [`Migrations/`](./Migrations/).

## Compiler et nettoyer

Compilation Release :

```bash
dotnet build -c Release
```

Nettoyage des fichiers générés :

```bash
dotnet clean
```

## Organisation du projet

```text
src/
├── Data/             Contexte EF Core
├── endpoints/        Routes Todo et génération JWT
├── models/           Modèles Todo et Token
├── services/         Interfaces et logique métier
└── test/             Requêtes HTTP manuelles
Migrations/           Historique du schéma SQLite
MonPremierProjet.Tests/ Tests automatisés
```

## Limites actuelles et prochaines étapes

Le projet est destiné à l'apprentissage. Les améliorations naturelles sont :

- [Étape 1 — DTO et validation](./docs/01-dto-et-validation.md)
- [Étape 2 — Gestion globale des erreurs](./docs/02-gestion-des-erreurs.md)
- [Étape 3 — Tests d'intégration HTTP](./docs/03-tests-integration.md)
- [Étape 4 — Recherche, filtrage et pagination](./docs/04-recherche-et-pagination.md)
- remplacer `/tokendev` par une vraie connexion utilisateur ;
- déplacer tous les secrets vers User Secrets ou des variables d'environnement.

Les guides sont à suivre dans l'ordre. Chaque fichier indique l'objectif, les
notions à comprendre, les modifications à réaliser et les vérifications à
effectuer avant de passer à l'étape suivante.
