# MonPremierProjet

Petite API ASP.NET Core en .NET 10, avec des endpoints Todo et des tests xUnit.

## Lancer l'API

```bash
dotnet restore
dotnet run
```

L'API est ensuite accessible à l'adresse indiquée dans la console.

## Tester l'API

Ouvrir `src/test/MonPremierProjet.http` dans VS Code, puis cliquer sur **Send Request** au-dessus d'une requête.

## Compiler

```bash
dotnet build -c Release
```

### Lancer via dll
```bash
dotnet ./bin/Release/net10.0/MonPremierProjet.dll
```

## Lancer les tests

```bash
dotnet test MonPremierProjet.slnx
```

## Ajouter un projet à la solution

```bash
dotnet sln MonPremierProjet.slnx add chemin/vers/projet.csproj
```

## Nettoyer les fichiers générés

```bash
dotnet clean
```

## Exercices guidés

### Exercice 1 — Persister les Todo avec SQLite et EF Core

Objectif : remplacer la liste en mémoire de `TodoService` par une base SQLite
locale, tout en conservant l'interface `ITodoService`.

1. Ajouter les dépendances :

   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package Microsoft.EntityFrameworkCore.Design
   dotnet tool install --global dotnet-ef
   ```

2. Créer `src/Data/AppDbContext.cs` :

   ```csharp
   using Microsoft.EntityFrameworkCore;
   using MonPremierProjet.Models;

   namespace MonPremierProjet.Data;

   public class AppDbContext(DbContextOptions<AppDbContext> options)
       : DbContext(options)
   {
       public DbSet<Todo> Todos => Set<Todo>();
   }
   ```

3. Ajouter une chaîne de connexion dans `appsettings.json` :

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=app.db"
   }
   ```

4. Enregistrer le contexte dans `src/Program.cs` :

   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlite(
           builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

   Ajouter aussi :

   ```csharp
   using Microsoft.EntityFrameworkCore;
   using MonPremierProjet.Data;
   ```

5. Modifier `TodoService` pour injecter `AppDbContext` et remplacer les
   opérations sur `List<Todo>` par `DbSet<Todo>`.

6. Faire évoluer `ITodoService` avec des méthodes asynchrones (`Task`,
   `ToListAsync`, `FindAsync`, `SaveChangesAsync`).

7. Créer puis appliquer la première migration :

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

8. Vérifier qu'un Todo est toujours présent après l'arrêt et le redémarrage
   de l'API. Le fichier `app.db` doit apparaître à la racine du projet.

Points à observer :

- `DbContext` est enregistré en `Scoped`, car il représente une session avec
  la base de données ;
- les méthodes EF asynchrones évitent de bloquer le serveur ;
- les migrations décrivent l'évolution du schéma.

### Exercice 2 — Protéger certaines routes avec un token Bearer

Objectif : rendre la lecture publique, mais exiger un token pour créer,
modifier ou supprimer un Todo. Cet exercice utilise un JWT local pour
comprendre le mécanisme ; ce n'est pas un système complet de gestion des
utilisateurs destiné à la production.

1. Ajouter le package :

   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   ```

2. Ajouter une clé de développement dans `appsettings.Development.json` :

   ```json
   "Jwt": {
     "Key": "cle-de-developpement-a-remplacer-par-une-valeur-secrete-longue",
     "Issuer": "MonPremierProjet"
   }
   ```

   Ne jamais committer une vraie clé. En pratique, utiliser un secret local ou
   une variable d'environnement.

3. Dans `Program.cs`, enregistrer l'authentification et l'autorisation :

   ```csharp
   using Microsoft.AspNetCore.Authentication.JwtBearer;
   using Microsoft.IdentityModel.Tokens;
   using System.Text;

   var jwtKey = builder.Configuration["Jwt:Key"]
       ?? throw new InvalidOperationException("Jwt:Key is missing.");

   builder.Services
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
               ValidateIssuer = true,
               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidateAudience = false,
               ValidateLifetime = true
           };
       });
   builder.Services.AddAuthorization();
   ```

4. Ajouter ces middlewares avant l'enregistrement des endpoints :

   ```csharp
   app.UseAuthentication();
   app.UseAuthorization();
   ```

5. Protéger les routes d'écriture dans `TodoEndpoint` :

   ```csharp
   app.MapPost("/todos", /* handler */)
       .RequireAuthorization();

   app.MapPut("/todos/{id}", /* handler */)
       .RequireAuthorization();

   app.MapDelete("/todos/{id}", /* handler */)
       .RequireAuthorization();
   ```

6. Créer un endpoint de développement qui fabrique un JWT avec
   `JwtSecurityTokenHandler`. Il doit utiliser la même clé et le même issuer,
   et ne doit pas être conservé tel quel en production.

7. Dans le fichier `.http`, appeler une route protégée sans token :

   ```http
   POST {{MonPremierProjet_HostAddress}}/todos
   Content-Type: application/json

   {
     "title": "Test sans token",
     "isCompleted": false
   }
   ```

   Résultat attendu : `401 Unauthorized`.

8. Refaire la requête avec le token obtenu :

   ```http
   POST {{MonPremierProjet_HostAddress}}/todos
   Authorization: Bearer {{accessToken}}
   Content-Type: application/json

   {
     "title": "Test avec token",
     "isCompleted": false
   }
   ```

   Résultat attendu : `201 Created`.

À retenir :

- `UseAuthentication` identifie l'appelant ;
- `UseAuthorization` vérifie qu'il a le droit d'accéder à la route ;
- `.RequireAuthorization()` protège une route précise ;
- un token absent ou invalide produit `401`, tandis qu'un utilisateur
  authentifié mais insuffisamment autorisé produit généralement `403`.
