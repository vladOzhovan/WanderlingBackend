using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wanderling.Infrastructure.Data;
using Wanderling.Infrastructure.Identity;


namespace Wanderling.Api.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWanderlingInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            var dbPath = Path.Combine(env.ContentRootPath, "..", "Wanderling.Infrastructure", "Wanderlings.db");
            var connectionString = $"Data Source={dbPath}";

            services.AddMemoryCache();
            services.AddDbContext<WanderlingDbContext>(options => options.UseSqlite(connectionString));

            services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
             .AddRoles<AppRole>()
             .AddEntityFrameworkStores<WanderlingDbContext>()
             .AddDefaultTokenProviders();

            services.AddAuthentication();
            services.AddAuthorization();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
