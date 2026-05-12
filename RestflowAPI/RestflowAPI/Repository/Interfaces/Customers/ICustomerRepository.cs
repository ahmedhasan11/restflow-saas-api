using RestflowAPI.DTOs.Customers;
using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.Repository.Interfaces.Customers
{
	public interface ICustomerRepository
	{
		Task<List<CustomerDto>> GetAllAsync(string? search , CustomerStatus? customerStatus, CancellationToken cancellationToken);
		Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, Guid? excludeId, CancellationToken cancellationToken);
		Task AddAsync(Customer customer);
		Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	}
}
