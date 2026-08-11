
using HrModule.Extensions;
using HrModule.Filters;
using HrModule.Middlewares;
using Infrastructure.Seeders;

namespace HrModule
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
            });
            builder.Services.AddPresentationServices(builder.Configuration);
            builder.Services.AddWebApiServices(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                if (app.Environment.IsProduction())
                {
                    c.DocumentTitle = "Production ERP API Docs";
                }
            });
            app.UseMiddleware<GlobalExceptionHandling>();
            app.UseHttpsRedirection();
            app.UseCors("AllowAllWithCredentials");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await AppSeeder.SeedAsync(services);
            }
            app.Run();
        }
    }
}
