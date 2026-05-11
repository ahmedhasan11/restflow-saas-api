using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Customers
{
	public interface ICustomerRepository
	{
		Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken);
		Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, Guid? excludeId, CancellationToken cancellationToken);
		Task AddAsync(Customer customer);
	}
}
