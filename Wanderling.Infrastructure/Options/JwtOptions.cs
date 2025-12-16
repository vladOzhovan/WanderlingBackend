namespace Wanderling.Infrastructure.Options
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string SecurityKey { get; init; } = string.Empty;
        public int AccessTokenLifetimeMinutes { get; init; } = 60 * 24 * 7;
    }
}
