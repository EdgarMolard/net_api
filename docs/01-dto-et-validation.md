# Étape 1 — DTO et validation

Cette étape consiste à séparer **ce que l'API reçoit et renvoie** de
**l'objet qu'Entity Framework Core enregistre en base**.

## 1. Comprendre les deux objets

Le projet contient déjà cette entité :

```csharp
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
```

`Todo` est un **modèle de persistence** :

- EF Core l'utilise pour représenter la table `Todos` ;
- `Id` est attribué par la base de données ;
- les services travaillent avec cet objet.

Un DTO (*Data Transfer Object*) est un objet dédié aux échanges HTTP. Il ne
représente pas une table. Il décrit uniquement les données qu'un client peut
envoyer ou recevoir.

Par exemple, lors d'un `POST`, le client ne doit pas envoyer un `Id` :

```json
{
  "title": "Acheter du lait",
  "isCompleted": false
}
```

Le flux attendu est donc :

```text
JSON HTTP
   ↓
CreateTodoRequest       (contrat d'entrée)
   ↓ conversion
Todo                    (entité interne)
   ↓
TodoService → base de données
   ↓ conversion
TodoResponse            (contrat de sortie)
   ↓
JSON HTTP
```

Même si deux classes ont des propriétés identiques, elles n'ont pas le même
rôle. Cette séparation permet de modifier la base de données sans modifier
automatiquement le contrat public de l'API.

## 2. Créer les contrats HTTP

Les contrats se trouvent dans `src/contracts/`. Créez trois classes séparées.
N'utilisez pas une classe générique `TodoDTO` pour les trois cas : une requête
de création, une requête de modification et une réponse n'ont pas exactement
la même responsabilité.

### `CreateTodoRequest`

Ce DTO représente le corps d'un `POST /todos`.

```csharp
using System.ComponentModel.DataAnnotations;

namespace MonPremierProjet.Contracts;

public class CreateTodoRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
```

`[Required]` refuse une valeur absente ou vide. `[MaxLength(100)]` refuse un
titre de plus de 100 caractères. Ces attributs décrivent les règles du
contrat HTTP ; ils ne remplacent pas la classe `Todo`.

### `UpdateTodoRequest`

Ce DTO représente le corps d'un `PUT /todos/{id}`. Pour cet exercice, il
contient les mêmes propriétés :

```csharp
using System.ComponentModel.DataAnnotations;

namespace MonPremierProjet.Contracts;

public class UpdateTodoRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
```

### `TodoResponse`

Ce DTO représente ce que l'API a le droit de renvoyer au client :

```csharp
namespace MonPremierProjet.Contracts;

public class TodoResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
```

## 3. Activer la validation automatique

Dans `src/Program.cs`, ajoutez l'enregistrement de la validation avec les
autres services :

```csharp
builder.Services.AddValidation();
```

L'objectif est que le framework inspecte les attributs `[Required]` et
`[MaxLength]` avant d'exécuter le handler. Si les données sont invalides, la
requête doit s'arrêter avec une réponse `400 Bad Request`.

## 4. Modifier le endpoint POST

Dans `src/endpoints/TodoEndpoint.cs` :

1. Importez `MonPremierProjet.Contracts`.
2. Remplacez le paramètre `Todo todo` par `CreateTodoRequest request`.
3. Supprimez l'appel à `ValidateTitle`.
4. Construisez explicitement une entité `Todo`.
5. Appelez le service avec cette entité.
6. Convertissez le résultat en `TodoResponse` avant de le renvoyer.

Le handler doit suivre cette idée :

```csharp
app.MapPost("/todos", async (
    CreateTodoRequest request,
    ITodoService service) =>
{
    var todo = new Todo
    {
        Title = request.Title,
        IsCompleted = request.IsCompleted
    };

    var createdTodo = await service.AddAsync(todo);

    var response = new TodoResponse
    {
        Id = createdTodo.Id,
        Title = createdTodo.Title,
        IsCompleted = createdTodo.IsCompleted
    };
    return Results.Created($"/todos/{createdTodo.Id}", response);
});
```

Le point important est la conversion explicite :

```text
CreateTodoRequest → Todo → TodoResponse
```

On ne renvoie pas directement `createdTodo`, car cela exposerait l'entité
interne comme réponse publique.

## 5. Modifier le endpoint PUT

Appliquez le même principe à `PUT /todos/{id}` :

1. Recevez un `UpdateTodoRequest`.
2. Créez un `Todo` à partir de ses propriétés.
3. Appelez `UpdateAsync`.
4. Si le résultat vaut `null`, renvoyez `404 Not Found`.
5. Sinon, convertissez le résultat en `TodoResponse`.

Le service peut continuer à travailler avec `Todo`. Le service n'a pas besoin
de connaître les DTO HTTP.

## 6. Vérifier progressivement

Après chaque modification, compilez le projet :

```bash
dotnet build
```

Puis lancez l'application et vérifiez les cas suivants avec le fichier
`src/test/MonPremierProjet.http` ou un client HTTP :

| Requête | Résultat attendu |
| --- | --- |
| `POST` avec un titre valide | `201 Created` |
| `POST` sans `Title` | `400 Bad Request` |
| `POST` avec un titre de plus de 100 caractères | `400 Bad Request` |
| `GET /todos` | uniquement `Id`, `Title`, `IsCompleted` |
| `PUT` avec un titre valide | `200 OK` |
| `PUT` avec un id inexistant | `404 Not Found` |

## À retenir

- `Todo` appartient au modèle interne et à la persistence.
- `CreateTodoRequest` et `UpdateTodoRequest` appartiennent aux entrées HTTP.
- `TodoResponse` appartient aux sorties HTTP.
- Un DTO peut ressembler à une entité sans être la même classe.
- Le mapping explicite entre les classes protège la frontière de l'API.
