using Microsoft.EntityFrameworkCore;
using Wanderling.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Wanderling.Infrastructure.Data
{
    public class WanderlingDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public WanderlingDbContext(DbContextOptions<WanderlingDbContext> options) : base(options)
        {
            
        }
    }
}
