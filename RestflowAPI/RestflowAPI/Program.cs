
using Microsoft.EntityFrameworkCore;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Tenants;

namespace RestflowAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
			// Add Tenant Services
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddScoped<ICurrentTenantService,CurrentTenantService>();
			// Add services to the container.
			// Configure Entity Framework Core with SQL Server
			builder.Services.AddDbContext<RestflowAPI.Data.ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			// Add our custom tenant validation middleware
			app.UseMiddleware<RestflowAPI.Middlewares.TenantValidationMiddleware>();
			app.MapControllers();

            app.Run();
        }
    }
}
