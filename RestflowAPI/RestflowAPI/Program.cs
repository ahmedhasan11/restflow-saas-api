
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.Repository.Auth;
using RestflowAPI.Repository.Tenants;
using RestflowAPI.RepositoryInterfaces.Auth;
using RestflowAPI.RepositoryInterfaces.Tenants;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Auth;
using RestflowAPI.Services.Tenants;
using RestflowAPI.Settings;
using System.Text;

namespace RestflowAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Configure JWT Settings
			var jwtSettings = new JwtSettings();
			builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
			builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
			// Add Tenant Services
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddScoped<IAuthRepository, AuthRepository>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<ICurrentTenantService,CurrentTenantService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IJwtService, JwtService>();
			builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
			builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			builder.Services.AddScoped<ITenantRepository, TenantRepository>();
			builder.Services.AddScoped<ITenantService, TenantService>();

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

				// Lockout Settings (US-20)
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;
			})
			.AddEntityFrameworkStores<RestflowAPI.Data.ApplicationDbContext>()
			.AddDefaultTokenProviders();

			// Configure JWT Authentication
			builder.Services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options => {
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
				};
			});


			builder.Services.AddControllers()
				.AddJsonOptions(options => {
					options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
					options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				});
			builder.Services.AddValidatorsFromAssemblyContaining<Program>();

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.SuperAdminOnly,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.SuperAdmin));

				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.OwnerOnly,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.Owner));

				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.EmployeeOnly,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.Employee));

				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.TenantAccess,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.Owner, RestflowAPI.Constants.Permissions.Roles.Employee));
			});
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

			// Seed Roles
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RestflowAPI.Entities.ApplicationRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RestflowAPI.Entities.ApplicationUser>>();
				var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
				await RestflowAPI.Data.IdentityDbInitializer.SeedRolesAsync(roleManager);
				await RestflowAPI.Data.IdentityDbInitializer.SeedSuperAdminAsync(userManager, configuration);
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
