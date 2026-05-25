using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Employees;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Repository.Employees
{
	public class EmployeesRepository : IEmployeesRepository
	{
		private readonly ApplicationDbContext _db;

		public EmployeesRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
		{
			await _db.Employees.AddAsync(employee, cancellationToken);
		}

		public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			return await _db.Employees
				.Where(e => e.Id == id)
				.Select(e => new EmployeeDto
				{
					Id = e.Id,
					FullName = e.FullName,
					Email = e.Email,
					PhoneNumber = e.PhoneNumber,
					Role = e.Role,
					Status = e.Status,
					CreatedAt = e.CreatedAt,
					UpdatedAt = e.UpdatedAt
				})
				.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<Employee?> GetEntityByIdAsync(Guid employeeId, CancellationToken cancellationToken)
		{
			return await _db.Employees
				.Include(e => e.User)
				.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
		}

		public async Task<List<EmployeeDto>> GetStaffListAsync(string? search, string? role, UserStatus? status, CancellationToken cancellationToken)
		{
			var query = _db.Employees.AsQueryable();

			if (status.HasValue)
			{
				query = query.Where(e => e.Status == status.Value);
			}
			if (!string.IsNullOrWhiteSpace(search))
			{
				var lowerSearch = search.ToLower();
				query = query.Where(e =>
					e.FullName.ToLower().Contains(lowerSearch) ||
					e.Email.ToLower().Contains(lowerSearch) ||
					e.PhoneNumber.Contains(lowerSearch));
			}

			if (!string.IsNullOrWhiteSpace(role))
			{
				query = query.Where(e => e.Role == role);
			}

			return await query
				.Select(e => new EmployeeDto
				{
					Id = e.Id,
					FullName = e.FullName,
					Email = e.Email,
					PhoneNumber = e.PhoneNumber,
					Role = e.Role,
					Status = e.Status,
					CreatedAt = e.CreatedAt,
					UpdatedAt = e.UpdatedAt
				})
				.OrderBy(e => e.CreatedAt)
				.ToListAsync(cancellationToken);

		}
	}
}
