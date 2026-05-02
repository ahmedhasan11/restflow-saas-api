using Microsoft.AspNetCore.Identity;
using RestflowAPI.Entities;

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
	}
}
