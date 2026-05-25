using FluentValidation;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Employees;
using RestflowAPI.Repository.Interfaces.Auth;
using RestflowAPI.Repository.Interfaces.Employees;
using RestflowAPI.ServiceInterfaces.Employees;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.Employees
{
	public class EmployeesService : IEmployeesService
	{
		private readonly IEmployeesRepository _employeesRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateEmployeeDto> _createEmployeeValidator;
		private readonly IAuthRepository _authRepository;
		private readonly ICurrentTenantService _tenantService;
		public EmployeesService(IEmployeesRepository employeesRepository, IUnitOfWork unitOfWork, IValidator<CreateEmployeeDto> createEmployeeValidator, IAuthRepository authRepository, ICurrentTenantService tenantService)
		{
			_employeesRepository = employeesRepository;
			_unitOfWork = unitOfWork;
			_createEmployeeValidator = createEmployeeValidator;
			_authRepository = authRepository;
			_tenantService = tenantService;
		}

		public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto request, CancellationToken cancellationToken)
		{
			var validationResult = await _createEmployeeValidator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			// 2. Globally Unique Email Check (ignoring query filters)
			var existingEmail = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (existingEmail != null)
			{
				throw new ConflictException("Email is already in use.");
			}
			// 3. Globally Unique Phone Check (ignoring query filters)
			var existingPhone = await _authRepository.FindByPhoneAsync(request.PhoneNumber, cancellationToken);
			if (existingPhone != null)
			{
				throw new ConflictException("Phone number is already in use.");
			}

			// 4. Create User linked to Current Tenant
			var user = new ApplicationUser
			{
				FullName = request.FullName,
				UserName = request.Email,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				TenantId = _tenantService.TenantId,
				Status = UserStatus.Active, // Default status: active
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				CreatedAt = DateTime.UtcNow
			};

			var createResult = await _authRepository.CreateUserAsync(user, request.Password);
			if (!createResult.Succeeded)
			{
				throw new AppValidationException(createResult.Errors.Select(e => e.Description));
			}
			// 5. Assign Role
			var roleResult = await _authRepository.AddToRoleAsync(user, request.Role);
			if (!roleResult.Succeeded)
			{
				throw new AppValidationException(roleResult.Errors.Select(e => e.Description));
			}

			// 6. Commit transaction
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new EmployeeDto
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber ?? string.Empty,
				Role = request.Role,
				Status = user.Status,
				CreatedAt = user.CreatedAt
			};
		}

		public async Task<EmployeeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			var employee = await _employeesRepository.GetByIdAsync(id, cancellationToken);
			if (employee == null)
			{
				throw new NotFoundException("Employee not found.");
			}
			return employee;
		}

		public async Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken)
		{
			return await _employeesRepository.GetStaffListAsync(cancellationToken);
		}
	}
}
