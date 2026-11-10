# Étape 2 — Gestion globale des erreurs

## Objectif

Retourner des erreurs HTTP cohérentes sans ajouter des `try/catch` dans chaque
endpoint.

## Notions à comprendre

- middleware ASP.NET Core ;
- `ProblemDetails` et le format RFC 7807 ;
- différence entre erreur prévue et exception inattendue ;
- journalisation avec `ILogger`.

## Travail guidé

1. Ajouter `builder.Services.AddProblemDetails()` dans `Program.cs`.
2. Ajouter `app.UseExceptionHandler()` dans le pipeline.
3. Ajouter `app.UseStatusCodePages()` pour les réponses d'erreur sans contenu.
4. Vérifier que les erreurs renvoient un JSON avec `status`, `title` et
   `instance`.
5. Remplacer progressivement les messages texte de validation par des erreurs
   structurées.
6. Ajouter un `ILogger` dans les services ou endpoints lorsque cela apporte
   une information utile.

## Vérifications

- une exception inattendue ne renvoie pas sa stack trace au client ;
- une erreur HTTP reste journalisée côté serveur ;
- un identifiant inexistant retourne `404` ;
- une donnée invalide retourne `400` avec une réponse structurée.

## À retenir

Le client reçoit une réponse sûre et prévisible. Les détails techniques
restent dans les logs du serveur.
