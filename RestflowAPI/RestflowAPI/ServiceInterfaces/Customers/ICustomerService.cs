using RestflowAPI.DTOs.Customers;

namespace RestflowAPI.ServiceInterfaces.Customers
{
	public interface ICustomerService
	{
		Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken);
		Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken);
	}
}
