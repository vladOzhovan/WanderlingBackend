namespace Wanderling.Application.Dtos
{
    public class AuthenticationDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
    }
}
