using FluentValidation;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Customers;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.Customers;
using RestflowAPI.ServiceInterfaces.Customers;
using RestflowAPI.Validators.CustomerValidators;

namespace RestflowAPI.Services.Customers
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IValidator<CreateCustomerDto> _createCustomerValidator;
		private readonly IUnitOfWork _unitOfWork;
		public CustomerService(ICustomerRepository customerRepository , IValidator<CreateCustomerDto> createCustomerValidator
			, IUnitOfWork unitOfWork)
		{
			_customerRepository = customerRepository;
			_createCustomerValidator = createCustomerValidator;
			_unitOfWork = unitOfWork;
		}

		public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken)
		{
			var result = await _createCustomerValidator.ValidateAsync(dto, cancellationToken);
			if (!result.IsValid)
			{
				throw new AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}
			if (await _customerRepository.ExistsByPhoneNumberAsync(dto.PhoneNumber, null, cancellationToken))
			{
				throw new ConflictException("A customer with this phone number already exists in this restaurant.");
			}

			var customer = new Customer
			{
				Id= Guid.NewGuid(),
				FullName = dto.FullName,
				PhoneNumber = dto.PhoneNumber,
				Status = CustomerStatus.Active
			};

			await _customerRepository.AddAsync(customer);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return CustomerResponseDto.Success(MapToCustomerDto(customer), "Customer created successfully.");

		}

		public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken)
		{
			var customers = await _customerRepository.GetAllAsync(cancellationToken);
			return customers.Select(c=> MapToCustomerDto(c)).ToList();
		}

		private static CustomerDto MapToCustomerDto(Customer customer)
		{
			return new CustomerDto
			{
				Id = customer.Id,
				FullName = customer.FullName,
				PhoneNumber = customer.PhoneNumber,
				Status = customer.Status
			};
		}
	}
}
