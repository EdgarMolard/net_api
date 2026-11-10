using MonPremierProjet.Models;

namespace MonPremierProjet.Services;

public interface ITokenBuilder
{
    Task<Token> BuildTokenAsync();
}