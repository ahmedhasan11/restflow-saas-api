using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Customers;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Customers;

namespace RestflowAPI.Repository.Customers
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly ApplicationDbContext _db;

		public CustomerRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task AddAsync(Customer customer)
		{
			 await _db.Customers.AddAsync(customer);
		}
		public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, Guid? excludeId, CancellationToken cancellationToken)
		{
			return await _db.Customers.AnyAsync(c=>c.PhoneNumber==phoneNumber && (excludeId==null || c.Id != excludeId), cancellationToken);
		}
		public async Task<List<CustomerDto>> GetAllAsync(string? search , CustomerStatus? customerStatus, CancellationToken cancellationToken)
		{
			var query = _db.Customers.AsQueryable();
			if (!string.IsNullOrWhiteSpace( search))
			{
				query=query.Where(c => c.FullName.Contains(search) || c.PhoneNumber.Contains(search));
			}
			if (!customerStatus.HasValue)
			{
				customerStatus = CustomerStatus.Active;
			}
			query = query.Where(c => c.Status == customerStatus.Value);

			return await query.OrderByDescending(c => c.CreatedAt).Select(c => new CustomerDto()
				{
					Id = c.Id,
					FullName = c.FullName,
					PhoneNumber = c.PhoneNumber,
					Status = c.Status,
					CreatedAt = c.CreatedAt,
					UpdatedAt = c.UpdatedAt
				}).ToListAsync(cancellationToken);

		}
		public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			return await _db.Customers.FirstOrDefaultAsync(c=>c.Id==id, cancellationToken);
		}
	}
}
