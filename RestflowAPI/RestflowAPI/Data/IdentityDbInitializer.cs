using Microsoft.AspNetCore.Identity;
using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.Data
{
	public static class IdentityDbInitializer
	{
		public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
		{
			string[] roles = { "SuperAdmin", "Owner", "Employee" };

			foreach (var roleName in roles)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new ApplicationRole(roleName));
				}
			}
		}
		public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			var adminEmail = configuration["SuperAdmin:Email"]; // Must be set in secrets.json
			var adminPassword = configuration["SuperAdmin:Password"]; // Must be set in secrets.json
			var adminPhone = configuration["SuperAdmin:Phone"]; // Must be set in secrets.json
			if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrWhiteSpace(adminPhone))
			{
				throw new InvalidOperationException("SuperAdmin credentials are missing in configuration. Ensure SuperAdmin:Email, SuperAdmin:Password, and SuperAdmin:Phone are supplied via secrets.json.");
			}

			var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
			if (existingAdmin == null)
			{
				var adminUser = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					PhoneNumber = adminPhone,
					FullName = "System Super Admin",
					Status = UserStatus.Active,
					EmailConfirmed = true,
					PhoneNumberConfirmed = true,
					CreatedAt = DateTime.UtcNow
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
				}
			}
		}
	}
}
