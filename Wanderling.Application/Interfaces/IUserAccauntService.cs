using Wanderling.Application.Dtos;
using Wanderling.Application.Results;

namespace Wanderling.Application.Interfaces
{
    public interface IUserAccauntService
    {
        Task<Result<RegistrationDto>> RegisterUserAsync(RegisterDto dto, CancellationToken ct = default);
        Task<Result<AuthenticationDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    }
}
