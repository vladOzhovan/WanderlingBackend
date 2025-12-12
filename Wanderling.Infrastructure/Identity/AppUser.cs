using Microsoft.AspNetCore.Identity;

namespace Wanderling.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
