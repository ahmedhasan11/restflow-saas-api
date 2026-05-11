using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;
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
		public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _db.Customers.OrderByDescending(c=>c.CreatedAt).ToListAsync(cancellationToken);
		}
	}
}
