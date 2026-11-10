# Étape 1 — DTO et validation

## Objectif

Ne plus utiliser directement `Todo` comme entrée et sortie HTTP. Les DTO
définissent le contrat de l'API et évitent d'exposer le modèle de persistence.

## Notions à comprendre

- différence entre une entité EF Core et un DTO ;
- séparation entre modèle interne et contrat HTTP ;
- validation des données entrantes ;
- réponses `400 Bad Request`.

## Travail guidé

1. Créer un dossier `src/contracts/`.
2. Créer `CreateTodoRequest` avec :
   - `Title` obligatoire ;
   - une longueur maximale de 100 caractères ;
   - `IsCompleted`.
3. Créer `UpdateTodoRequest` avec les mêmes propriétés.
4. Créer `TodoResponse` avec `Id`, `Title` et `IsCompleted`.
5. Modifier les routes POST et PUT pour recevoir les DTO.
6. Convertir les DTO en `Todo` avant d'appeler le service.
7. Convertir les `Todo` en `TodoResponse` avant de répondre.
8. Remplacer la validation manuelle par la validation .NET.
9. Ajouter `builder.Services.AddValidation()` dans `Program.cs`.

## Vérifications

- un POST valide retourne `201 Created` ;
- un titre vide retourne `400 Bad Request` ;
- un titre de plus de 100 caractères retourne `400 Bad Request` ;
- les réponses ne contiennent pas de propriétés inconnues de `TodoResponse`.

## À retenir

Le DTO appartient à l'API. L'entité appartient à la persistence. Les séparer
permet de faire évoluer la base de données sans casser les clients HTTP.
