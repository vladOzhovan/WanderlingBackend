using Microsoft.AspNetCore.Identity;

namespace Wanderling.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
