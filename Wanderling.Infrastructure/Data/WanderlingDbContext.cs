using Microsoft.EntityFrameworkCore;
using Wanderling.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Wanderling.Infrastructure.Data
{
    public class WanderlingDbContext : IdentityDbContext<AppUser>
    {
        public WanderlingDbContext(DbContextOptions<WanderlingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "ROLE_ADMIN",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "ROLE_MODERATOR",
                    Name = "Moderator",
                    NormalizedName = "MODERATOR"
                },
                new IdentityRole
                {
                    Id = "ROLE_PLAYER",
                    Name = "Player",
                    NormalizedName = "PLAYER"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
