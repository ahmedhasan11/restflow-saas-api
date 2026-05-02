using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Middlewares
{
	public class TenantValidationMiddleware
	{
		private readonly RequestDelegate _next;

		public TenantValidationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService, ApplicationDbContext dbContext)
		{
			var tenantId = tenantService.TenantId;

			// Only validate if a TenantId was actually provided in the request
			if (tenantId.HasValue)
			{
				var tenant = await dbContext.Tenants
					.AsNoTracking()
					.FirstOrDefaultAsync(t => t.Id == tenantId.Value);

				if (tenant == null)
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					await context.Response.WriteAsJsonAsync(new { Message = "The specified Tenant does not exist." });
					return; // Stop processing the request
				}

				if (tenant.Status != TenantStatus.Active)
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					await context.Response.WriteAsJsonAsync(new { Message = $"This Tenant is currently {tenant.Status} and cannot accept requests." });
					return; // Stop processing the request
				}
			}

			// If everything is good (or no tenant was passed), continue to the next step
			await _next(context);
		}
	}
}
