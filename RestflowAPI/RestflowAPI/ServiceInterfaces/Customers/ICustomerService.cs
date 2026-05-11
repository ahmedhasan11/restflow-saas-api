using RestflowAPI.DTOs.Customers;

namespace RestflowAPI.ServiceInterfaces.Customers
{
	public interface ICustomerService
	{
		Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken);
		Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken);
		Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

		Task<CustomerResponseDto> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken cancellationToken);

		Task<CustomerResponseDto> UpdateStatusAsync(Guid id, UpdateCustomerStatusDto dto, CancellationToken cancellationToken);
	}
}
