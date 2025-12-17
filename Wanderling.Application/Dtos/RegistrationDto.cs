namespace Wanderling.Application.Dtos
{
    public class RegistrationDto
    {
        public string UserId { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
    }
}
