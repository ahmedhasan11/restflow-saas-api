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

				// Try from header first
				if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantHeader))
				{
					if (Guid.TryParse(tenantHeader.ToString(), out var tenantId))
					{
						return tenantId;
					}
				}

				// Optionally try from User Claims (JWT) if authenticated
				var claim = context.User.FindFirst("TenantId")?.Value;
				if (claim != null && Guid.TryParse(claim, out var tenantIdFromClaim))
				{
					return tenantIdFromClaim;
				}

				return null;
			}
		}
	}
}
