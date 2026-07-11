using Data.Entities;

namespace Api.Auth.Services;

public interface IJwtTokenService
{
    public string CreateToken(Player player);
}