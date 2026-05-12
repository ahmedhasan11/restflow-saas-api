using FluentValidation;
using Org.BouncyCastle.Asn1.X509;
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
		private readonly IValidator<UpdateCustomerDto> _updateCustomerValidator;
		private readonly IValidator<UpdateCustomerStatusDto> _updateCustomerStatusValidator;
		private readonly IUnitOfWork _unitOfWork;
		public CustomerService(ICustomerRepository customerRepository , IValidator<CreateCustomerDto> createCustomerValidator
			, IValidator<UpdateCustomerDto> updateCustomerValidator, IValidator<UpdateCustomerStatusDto> updateCustomerStatusValidator, IUnitOfWork unitOfWork)
		{
			_customerRepository = customerRepository;
			_createCustomerValidator = createCustomerValidator;
			_updateCustomerValidator = updateCustomerValidator;
			_updateCustomerStatusValidator = updateCustomerStatusValidator;
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
		public async Task<List<CustomerDto>> GetAllAsync(string? search, CustomerStatus? customerStatus, CancellationToken cancellationToken)
		{
			return await _customerRepository.GetAllAsync(search, customerStatus, cancellationToken);
		}
		private static CustomerDto MapToCustomerDto(Customer customer)
		{
			return new CustomerDto
			{
				Id = customer.Id,
				FullName = customer.FullName,
				PhoneNumber = customer.PhoneNumber,
				Status = customer.Status,
				CreatedAt = customer.CreatedAt,
				UpdatedAt = customer.UpdatedAt
			};
		}
		public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
			if (customer==null)
			{
				throw new NotFoundException($"Customer with ID {id} not found.");
			}
			return MapToCustomerDto(customer);
		}
		public async Task<CustomerResponseDto> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken cancellationToken)
		{
			var result = await _updateCustomerValidator.ValidateAsync(dto, cancellationToken);
			if (!result.IsValid)
			{
				throw new AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}

			var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
			if (customer == null)
			{
				throw new NotFoundException($"Customer with ID {id} not found.");
			}

			if (dto.FullName != null)
				customer.FullName = dto.FullName;

			if (dto.PhoneNumber != null)
				customer.PhoneNumber = dto.PhoneNumber;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return CustomerResponseDto.Success(MapToCustomerDto(customer), "Customer updated successfully.");
		}
		public async Task<CustomerResponseDto> UpdateStatusAsync(Guid id, UpdateCustomerStatusDto dto, CancellationToken cancellationToken)
		{
			var validationResult = await _updateCustomerStatusValidator.ValidateAsync(dto, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}
			// 1. Existence Check (and Tenant isolation via filter)
			var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
			if (customer == null)
				throw new NotFoundException($"Customer with ID {id} not found.");

			// 2. Update status
			customer.Status = dto.Status;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			string message = dto.Status == CustomerStatus.Active ? "Customer activated." : "Customer deactivated.";
			return CustomerResponseDto.Success(MapToCustomerDto(customer), message);
		}
	}
}
