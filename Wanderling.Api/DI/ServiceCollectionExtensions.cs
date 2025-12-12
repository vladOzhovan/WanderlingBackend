using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wanderling.Infrastructure.Data;
using Wanderling.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace Wanderling.Api.DI
{
    // Extension methods for IServiceCollection to add Wanderling infrastructure services
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWanderlingInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            var dbPath = Path.Combine(env.ContentRootPath, "..", "Wanderling.Infrastructure", "Wanderlings.db");
            var connectionString = $"Data Source={dbPath}";

            services.AddMemoryCache();
            services.AddDbContext<WanderlingDbContext>(options => options.UseSqlite(connectionString));

            // Configure Identity
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = configuration["Jwt:Issuer"],
                     ValidAudience = configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                     RoleClaimType = System.Security.Claims.ClaimTypes.Role
                 };
             });

            services.AddAuthorization();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
