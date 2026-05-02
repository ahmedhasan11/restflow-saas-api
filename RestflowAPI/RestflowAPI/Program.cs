
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Tenants;

namespace RestflowAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
			// Add Tenant Services
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddScoped<ICurrentTenantService,CurrentTenantService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            
			// Add services to the container.
			// Configure Entity Framework Core with SQL Server
			builder.Services.AddDbContext<RestflowAPI.Data.ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			// Add Identity
			builder.Services.AddIdentity<RestflowAPI.Entities.ApplicationUser, RestflowAPI.Entities.ApplicationRole>(options => {
				options.Password.RequiredLength = 5;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;//symbols
				options.Password.RequireLowercase = true;
				options.Password.RequireDigit = false;
				//options.Password.RequiredUniqueChars = 3;
			})
			.AddEntityFrameworkStores<RestflowAPI.Data.ApplicationDbContext>()
			.AddDefaultTokenProviders();
			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

			// Seed Roles
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RestflowAPI.Entities.ApplicationRole>>();
				await RestflowAPI.Data.IdentityDbInitializer.SeedRolesAsync(roleManager);
			}
			// Configure the HTTP request pipeline.
			app.UseMiddleware<RestflowAPI.Middlewares.GlobalExceptionMiddleware>();

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

            await app.RunAsync();
        }
    }
}
