using Wanderling.Application.Dtos;

namespace Wanderling.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(TokenDto dto);
    }
}
