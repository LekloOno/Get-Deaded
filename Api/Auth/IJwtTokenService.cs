using Data.Entities;

namespace Api.Auth;

public interface IJwtTokenService
{
    public string CreateToken(Player player);
}