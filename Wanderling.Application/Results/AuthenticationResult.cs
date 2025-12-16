using Wanderling.Application.Dtos;

namespace Wanderling.Application.Results
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
        public AuthenticationDto? Data { get; init; }

        public static AuthenticationResult Ok(AuthenticationDto dto) => new() { Succeeded = true, Data = dto };
        public static AuthenticationResult Fail(params string[] errors) => new() { Succeeded = false, Errors = errors };
    }
}
