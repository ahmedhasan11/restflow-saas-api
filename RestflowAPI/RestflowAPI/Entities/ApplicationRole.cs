using Microsoft.AspNetCore.Identity;

namespace RestflowAPI.Entities
{
	public class ApplicationRole:IdentityRole<Guid>
	{
		public ApplicationRole() : base() { }

		public ApplicationRole(string roleName) : base(roleName) { }
	}
}
