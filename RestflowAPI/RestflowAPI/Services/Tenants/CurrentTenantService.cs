using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.Tenants
{
	public class CurrentTenantService : ICurrentTenantService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Guid? TenantId
		{
			get
			{
				var context = _httpContextAccessor.HttpContext;
				if (context == null) return null;

				// 1. ALWAYS prioritize the JWT Claim if the user is authenticated.
				// This prevents a logged-in user from spoofing another tenant via headers.
				var claim = context.User.FindFirst("TenantId")?.Value;
				if (claim != null && Guid.TryParse(claim, out var tenantIdFromClaim))
				{
					return tenantIdFromClaim;
				}

				// 2. Fallback to header ONLY if there is no authenticated user (e.g., Public Endpoints)
				if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantHeader))
				{
					if (Guid.TryParse(tenantHeader.ToString(), out var tenantId))
					{
						return tenantId;
					}
				}



				return null;
			}
		}
	}
}
