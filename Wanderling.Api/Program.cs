using Wanderling.Api.DI;
using Wanderling.Api.Middlewares;

namespace Wanderling.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddWanderlingInfrastructure(builder.Configuration, builder.Environment);

            var app = builder.Build();

            app.UseGlobalExceptionHandling();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
