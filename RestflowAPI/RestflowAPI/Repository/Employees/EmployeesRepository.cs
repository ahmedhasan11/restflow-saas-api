using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.Repository.Interfaces.Employees;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Repository.Employees
{
	public class EmployeesRepository : IEmployeesRepository
	{
		private readonly ApplicationDbContext _db;
		private readonly ICurrentTenantService _tenantService;
		public EmployeesRepository(ApplicationDbContext db, ICurrentTenantService tenantService)
		{
			_db = db;
			_tenantService = tenantService;
		}
		public async Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken)
		{
			var tenantId = _tenantService.TenantId;
			return await _db.Users.Where(u => u.TenantId == tenantId)
				.Select(u => new EmployeeDto
				{
					Id = u.Id,
					FullName = u.FullName,
					Email = u.Email ?? string.Empty,
					PhoneNumber = u.PhoneNumber ?? string.Empty,
					Status = u.Status,
					CreatedAt = u.CreatedAt,
					Role = _db.UserRoles
						.Where(ur => ur.UserId == u.Id)
						.Join(
							_db.Roles,
							ur => ur.RoleId,
							r => r.Id,
							(ur, r) => r.Name
						)
						.FirstOrDefault() ?? string.Empty
				})
				.OrderBy(e => e.CreatedAt)
				.ToListAsync(cancellationToken);
		}
	}
	
}
