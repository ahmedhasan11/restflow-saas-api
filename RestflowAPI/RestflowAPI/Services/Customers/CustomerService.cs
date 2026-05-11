using RestflowAPI.DTOs.Customers;
using RestflowAPI.Repository.Interfaces.Customers;
using RestflowAPI.ServiceInterfaces.Customers;

namespace RestflowAPI.Services.Customers
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;
		public CustomerService(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}
		public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken)
		{
			var customers = await _customerRepository.GetAllAsync(cancellationToken);
			return customers.Select(c => new CustomerDto
			{
				Id = c.Id,
				FullName = c.FullName,
				PhoneNumber = c.PhoneNumber,
				Status = c.Status
			}).ToList();
		}
	}
}
