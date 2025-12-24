using Domain.models;
using Infrastructure.user.Dto;

namespace ClinikTime.service.Auth;

public interface IAuthService
{
    public Task<Utilisateur?> LoginAsync(LoginUserDto dto);
}