# Étape 3 — Tests d'intégration HTTP

## Objectif

Tester l'API complète, et pas seulement `TodoService`. Ces tests vérifient le
routage, l'authentification, la sérialisation et la base de données ensemble.

## Notions à comprendre

- différence entre test unitaire et test d'intégration ;
- `WebApplicationFactory` ;
- `HttpClient` ;
- isolation de la base de test ;
- vérification des codes HTTP.

## Travail guidé

1. Ajouter le package de test ASP.NET Core compatible avec .NET 10.
2. Créer une factory de test qui démarre l'application.
3. Remplacer la connexion SQLite de production par une base SQLite dédiée aux
   tests.
4. Créer un test qui vérifie que `GET /todos` retourne `200`.
5. Créer un test qui vérifie que `POST /todos` sans token retourne `401`.
6. Appeler `/tokendev` depuis un test et récupérer `accessToken`.
7. Refaire le POST avec `Authorization: Bearer <token>` et vérifier `201`.
8. Ajouter les cas `404`, titre vide et titre trop long.

## Vérifications

- les tests sont indépendants et peuvent être exécutés dans n'importe quel
  ordre ;
- aucune donnée de test n'est écrite dans `app.db` ;
- les tests couvrent les routes publiques et protégées.

## À retenir

Un test unitaire vérifie une classe. Un test d'intégration vérifie que
plusieurs composants fonctionnent ensemble comme un utilisateur réel.
