using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.Repository.Interfaces.Employees;

namespace RestflowAPI.Repository.Employees
{
	public class EmployeesRepository : IEmployeesRepository
	{
		private readonly ApplicationDbContext _db;
		public EmployeesRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken)
		{
			return await _db.Users
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
