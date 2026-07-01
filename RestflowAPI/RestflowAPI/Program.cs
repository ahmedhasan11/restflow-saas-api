
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.Repository.Auth;
using RestflowAPI.Repository.Interfaces.Auth;
using RestflowAPI.Repository.Interfaces.Tenants;
using RestflowAPI.Repository.Tenants;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.ServiceInterfaces.ImenuCategory;
using RestflowAPI.ServiceInterfaces.ProductIngredient;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Auth;
using RestflowAPI.Services.Tenants;
using RestflowAPI.Settings;
using System.Text;
using RestflowAPI.swagger;
using System.Runtime.InteropServices;
using RestflowAPI.Repository.Interfaces.Customers;
using RestflowAPI.Repository.Customers;
using RestflowAPI.ServiceInterfaces.Customers;
using RestflowAPI.Services.Customers;
using RestflowAPI.Repository.Interfaces.Settings;
using RestflowAPI.Repository.Settings;
using RestflowAPI.ServiceInterfaces.Settings;
using RestflowAPI.Services.Settings;
using RestflowAPI.Repository.Interfaces.InventoryItem;
using RestflowAPI.Repository.InventoryItem;
using RestflowAPI.ServiceInterfaces.InventoryCategory;
using RestflowAPI.ServiceInterfaces.InventoryItems;
using RestflowAPI.Services.InventoryItems;
using RestflowAPI.Services;
using RestflowAPI.Repository.Interfaces.StockTransaction;
using RestflowAPI.ServiceInterfaces.StockTransaction;
using RestflowAPI.Services.StockTransaction;
using RestflowAPI.Repository.StockTransaction;
using RestflowAPI.Repository.Interfaces.Orders;
using RestflowAPI.Repository.Orders;
using RestflowAPI.ServiceInterfaces.Orders;
using RestflowAPI.Services.Orders;
using RestflowAPI.ServiceInterfaces.Employees;
using RestflowAPI.Services.Employees;
using RestflowAPI.Repository.Interfaces.Employees;
using RestflowAPI.Repository.Employees;
using RestflowAPI.ServiceInterfaces.Reports;
using RestflowAPI.Services.Reports;
using RestflowAPI.Repository.Interfaces.Reports;
using RestflowAPI.Repository.Reports;
using RestflowAPI.ServiceInterfaces.AI;
using RestflowAPI.Services.AI;
using RestflowAPI.Repository.AI;
using RestflowAPI.Repository.Interfaces.AI;


namespace RestflowAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			#region Configure JWT Settings
			var jwtSettings = new JwtSettings();
			builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
			builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

			builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

			builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
			#endregion

			builder.Services.AddHttpContextAccessor();

			#region Services Registration
			builder.Services.AddScoped<IAuthRepository, AuthRepository>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<ICurrentTenantService,CurrentTenantService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

			builder.Services.AddScoped<IJwtService, JwtService>();
			builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
			builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			builder.Services.AddScoped<ITenantRepository, TenantRepository>();
			builder.Services.AddScoped<ITenantService, TenantService>();
			builder.Services.AddScoped<IEmailService, EmailService>();
			builder.Services.AddScoped<ISmsService, SmsService>();

			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<IProductIngredientService, ProductIngredientService>();
			builder.Services.AddScoped<IMenuCategoryService, MenuCategoryService>();
			builder.Services.AddScoped<IProductIngredientRepository, ProductIngredientRepository>();
			builder.Services.AddScoped<IProductRepository, ProductRepository>();
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

			builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
			builder.Services.AddScoped<ICustomerService, CustomerService>();

			builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
			builder.Services.AddScoped<ISettingsService, SettingsService>();

			builder.Services.AddScoped<IFileService, FileService>();
			builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

			builder.Services.AddScoped<IEmployeesService, EmployeesService>();
			builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();

            builder.Services.AddScoped<IInventoryCategoryRepository, InventoryCategoryRepository>();
            builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();

            builder.Services.AddScoped<IInventoryCategoryService, InventoryCategoryService>();
            builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();

			builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            builder.Services.AddScoped<IStockMovementService, StockMovementService>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemsRepository, OrderItemRepository>();

            builder.Services.AddScoped<IOrdersService, OrdersService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();

            builder.Services.AddSingleton<ILogger>(sp =>
    sp.GetRequiredService<ILogger<OrdersService>>());

			builder.Services.AddScoped<IReportsService, ReportsService>();
			builder.Services.AddScoped<IReportsRepository, ReportsRepository>();

            builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Gemini"));

            builder.Services.AddHttpClient<ILLMService, GeminiService>();

            builder.Services.AddScoped<SchemaContextBuilder>();

            builder.Services.AddScoped<PromptBuilder>();

            builder.Services.AddScoped<ISqlValidationService, SqlValidationService>();

            builder.Services.AddScoped<IDynamicQueryRepository, DynamicQueryRepository>();

            builder.Services.AddScoped<IResponseSynthesisService, ResponseSynthesisService>();

            builder.Services.AddScoped<AnswerPromptBuilder>();

            builder.Services.AddScoped<IAIChatService, AIChatService>();

            builder.Services.AddScoped<IDashboardPromptBuilder, DashboardPromptBuilder>();

            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

            builder.Services.AddScoped<IDashboardInsightsService, DashboardInsightsService>();
            builder.Services.AddScoped<ISqlGenerationService, SqlGenerationService>();
            builder.Services.AddScoped<GeminiService>();

            #endregion

            #region Fluent Validation Configuration
            //Add Fluent Validations
            //builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
			#endregion
			#region DbContext Configuration with Identity


			builder.Services.AddDbContext<RestflowAPI.Data.ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.AddIdentity<RestflowAPI.Entities.ApplicationUser, RestflowAPI.Entities.ApplicationRole>(options => {
				options.Password.RequiredLength = 5;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;//symbols
				options.Password.RequireLowercase = true;
				options.Password.RequireDigit = false;



				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;
			})
			.AddEntityFrameworkStores<RestflowAPI.Data.ApplicationDbContext>()
			.AddDefaultTokenProviders();

			#endregion


			#region Authentication Configuration with JWT Bearer


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
			#endregion


			#region configure json requests to treat enums as strings not ints and to ignore case sensitivity in property names
			builder.Services.AddControllers()
				.AddJsonOptions(options => {
					options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
					options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				});
			#endregion



			#region Authorizaqtion Policies Configuration

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.SuperAdminOnly,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.SuperAdmin));

				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.OwnerOnly,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.Owner));

				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.EmployeeOnly,
					policy => policy.RequireRole(RestflowAPI.Constants.Permissions.Roles.Employee));

				options.AddPolicy(RestflowAPI.Constants.Permissions.Policies.TenantAccess,
					policy => policy.RequireRole(
						RestflowAPI.Constants.Permissions.Roles.Owner,
						RestflowAPI.Constants.Permissions.Roles.Employee,
						RestflowAPI.Constants.Permissions.Roles.Cashier,
						RestflowAPI.Constants.Permissions.Roles.Manager,
						RestflowAPI.Constants.Permissions.Roles.KitchenStaff,
						RestflowAPI.Constants.Permissions.Roles.InventoryClerk));
			});
			#endregion


			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.OperationFilter<TenantHeaderOperationFilter>();
			});

			var app = builder.Build();


			#region Seed Roles and Super Admin User
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RestflowAPI.Entities.ApplicationRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RestflowAPI.Entities.ApplicationUser>>();
				var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
				await RestflowAPI.Data.IdentityDbInitializer.SeedRolesAsync(roleManager);
				await RestflowAPI.Data.IdentityDbInitializer.SeedSuperAdminAsync(userManager, configuration);
			}
			#endregion
			// Configure the HTTP request pipeline.
			app.UseMiddleware<RestflowAPI.Middlewares.GlobalExceptionMiddleware>();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseAuthentication();
			app.UseAuthorization();

			// Add our custom tenant validation middleware
			app.UseMiddleware<RestflowAPI.Middlewares.TenantValidationMiddleware>();
			app.MapControllers();

            await app.RunAsync();
        }
    }
}
