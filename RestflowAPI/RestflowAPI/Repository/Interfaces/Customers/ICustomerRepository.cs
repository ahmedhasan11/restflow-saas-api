using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Customers
{
	public interface ICustomerRepository
	{
		Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken);
	}
}
