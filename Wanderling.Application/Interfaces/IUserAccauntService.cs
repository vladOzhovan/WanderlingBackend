using Wanderling.Application.Dtos;
using Wanderling.Application.Results;

namespace Wanderling.Application.Interfaces
{
    public interface IUserAccauntService
    {
        Task<AuthenticationResult> RegisterUserAsync(RegisterDto dto, CancellationToken ct = default);
        Task<AuthenticationResult> LoginAsync(LoginDto dto, CancellationToken ct = default);
    }
}
