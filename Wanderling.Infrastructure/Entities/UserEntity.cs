using Microsoft.AspNetCore.Identity;

namespace Wanderling.Infrastructure.Entities
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
