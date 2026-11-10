using MonPremierProjet.Services;
using MonPremierProjet.Models;
namespace MonPremierProjet.Endpoints;

public static class TokenBuilderEndpoint
{
    public static void TokenBuilderMap(this WebApplication app)
    {
        app.MapGet("/tokendev", async (ITokenBuilder service) =>
        {
            var token = await service.BuildTokenAsync();
            return Results.Ok(token);
        });
    }
}