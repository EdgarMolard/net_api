# Étape 4 — Recherche, filtrage et pagination

## Objectif

Faire évoluer `GET /todos` pour éviter de retourner toute la table lorsque le
nombre de Todo augmente.

## Notions à comprendre

- paramètres de query string ;
- pagination avec `page` et `pageSize` ;
- filtrage et recherche ;
- tri stable ;
- `Skip`, `Take`, `Where` et `OrderBy` avec EF Core.

## Travail guidé

1. Définir les paramètres :

   ```text
   GET /todos?page=1&pageSize=10&completed=false&search=api
   ```

2. Fixer une valeur par défaut pour `page` et `pageSize`.
3. Refuser les pages inférieures à 1.
4. Limiter la taille maximale d'une page.
5. Ajouter le filtre `completed`.
6. Ajouter la recherche sur `Title`.
7. Trier les résultats avant `Skip` et `Take`.
8. Retourner les métadonnées :
   - page courante ;
   - taille de page ;
   - nombre total de résultats ;
   - éléments de la page.
9. Ajouter des tests pour chaque combinaison importante.

## Vérifications

- la réponse ne retourne jamais plus que `pageSize` éléments ;
- la page suivante ne répète pas la page précédente ;
- une recherche vide retourne une liste vide avec `200` ;
- les valeurs invalides retournent `400`.

## À retenir

La pagination doit être faite dans la requête SQL, avant de charger les
données en mémoire.
