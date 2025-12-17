using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using Wanderling.Application.Interfaces;
using Wanderling.Infrastructure.Data;
using Wanderling.Infrastructure.Identity;
using Wanderling.Infrastructure.Options;
using Wanderling.Infrastructure.Services;


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
             .AddRoles<IdentityRole>()
             .AddEntityFrameworkStores<WanderlingDbContext>()
             .AddSignInManager<SignInManager<AppUser>>()
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
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"])),
                     RoleClaimType = ClaimTypes.Role
                 };
             });

            services.AddAuthorization();

            services.AddScoped<IUserAccauntService, UserAccauntService>();
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SupportNonNullableReferenceTypes();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Wanderling API", Version = "v1" });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
                });
            });

            return services;
        }
    }
}
