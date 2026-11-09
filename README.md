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
dotnet build
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
