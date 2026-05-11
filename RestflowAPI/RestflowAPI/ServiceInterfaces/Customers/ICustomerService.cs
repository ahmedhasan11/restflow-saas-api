using RestflowAPI.DTOs.Customers;

namespace RestflowAPI.ServiceInterfaces.Customers
{
	public interface ICustomerService
	{
		Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken);
	}
}
