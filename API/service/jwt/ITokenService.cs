using Domain.models;

namespace ClinikTime.service.jwt;

public interface ITokenService
{
    string GenerateToken(Utilisateur utilisateur);
}