using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
		private	readonly IValidator<UpdateEmployeeDto> _updateEmployeeValidator;
		private readonly IValidator<UpdateEmployeeStatusDto> _statusValidator;
		private readonly IAuthRepository _authRepository;
		private readonly ICurrentTenantService _tenantService;
		private readonly UserManager<ApplicationUser> _userManager;

		public EmployeesService(IEmployeesRepository employeesRepository, IUnitOfWork unitOfWork, 
			IValidator<CreateEmployeeDto> createEmployeeValidator, IAuthRepository authRepository,
			ICurrentTenantService tenantService, IValidator<UpdateEmployeeDto> updateEmployeeValidator,
			UserManager<ApplicationUser> userManager, IValidator<UpdateEmployeeStatusDto> statusValidator)
		{
			_employeesRepository = employeesRepository;
			_unitOfWork = unitOfWork;
			_createEmployeeValidator = createEmployeeValidator;
			_authRepository = authRepository;
			_tenantService = tenantService;
			_updateEmployeeValidator = updateEmployeeValidator;
			_userManager = userManager;
			_statusValidator = statusValidator;
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


			var tenantId = _tenantService.TenantId;
			if (tenantId == null || tenantId == Guid.Empty)
			{
				throw new AppValidationException("Tenant context is required.");
			}

			var user = new ApplicationUser
			{
				FullName = request.FullName,
				UserName = request.Email,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				TenantId = tenantId.Value,
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
			// 6. Create domain Employee entity record
			var employee = new Employee
			{
				Id = Guid.NewGuid(),
				UserId = user.Id,
				TenantId = tenantId.Value,
				FullName = request.FullName,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				Role = request.Role,
				Status = UserStatus.Active
			};
			await _employeesRepository.AddAsync(employee, cancellationToken);

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new EmployeeDto
			{
				Id = employee.Id, // Returning Employee.Id for business operational tracking
				FullName = employee.FullName,
				Email = employee.Email,
				PhoneNumber = employee.PhoneNumber,
				Role = employee.Role,
				Status = employee.Status,
				CreatedAt = employee.CreatedAt
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

		public async Task<List<EmployeeDto>> GetStaffListAsync(string? search, string? role, UserStatus? status, CancellationToken cancellationToken)
		{
			return await _employeesRepository.GetStaffListAsync(search, role, status, cancellationToken);
		}

		public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto request, CancellationToken cancellationToken)
		{
			// 1. Fluent Validation
			var validationResult = await _updateEmployeeValidator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			// 2. Fetch Employee within the owner's active tenant scope
			var employee = await _employeesRepository.GetEntityByIdAsync(id, cancellationToken);

			if (employee == null)
			{
				throw new NotFoundException("Employee not found.");
			}
			var user = employee.User;
			// 3. Unique Email Check (if changed)
			// 3. Unique Email Check (if changed)
			if (request.Email != null && !string.Equals(employee.Email, request.Email, StringComparison.OrdinalIgnoreCase))
			{
				var existingEmail = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
				if (existingEmail != null)
				{
					throw new ConflictException("Email is already in use.");
				}
				employee.Email = request.Email;
				if (user != null)
				{
					user.Email = request.Email;
					user.UserName = request.Email;
					user.NormalizedEmail = request.Email.ToUpper();
					user.NormalizedUserName = request.Email.ToUpper();
				}
			}
			// 4. Unique Phone Check (if changed)
			if (request.PhoneNumber != null && employee.PhoneNumber != request.PhoneNumber)
			{
				var existingPhone = await _authRepository.FindByPhoneAsync(request.PhoneNumber, cancellationToken);
				if (existingPhone != null)
				{
					throw new ConflictException("Phone number is already in use.");
				}
				employee.PhoneNumber = request.PhoneNumber;
				if (user != null)
				{
					user.PhoneNumber = request.PhoneNumber;
				}
			}
			// 5. Update other fields
			if (request.FullName != null)
			{
				employee.FullName = request.FullName;
				if (user != null)
				{
					user.FullName = request.FullName;
				}
			}

			if (request.Status != null)
			{
				employee.Status = request.Status.Value;
				if (user != null)
				{
					user.Status = request.Status.Value;
				}
			}
			// 6. Update Role (if changed)
			if (request.Role != null)
			{
				employee.Role = request.Role;
				if (user != null)
				{
					var currentRoles = await _userManager.GetRolesAsync(user);
					if (!currentRoles.Contains(request.Role))
					{
						var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
						if (!removeResult.Succeeded)
						{
							throw new AppValidationException(removeResult.Errors.Select(e => e.Description));
						}
						var addResult = await _userManager.AddToRoleAsync(user, request.Role);
						if (!addResult.Succeeded)
						{
							throw new AppValidationException(addResult.Errors.Select(e => e.Description));
						}
					}
				}
			}

			employee.UpdatedAt = DateTime.UtcNow;
			if (user != null)
			{
				user.UpdatedAt = DateTime.UtcNow;
				var updateResult = await _userManager.UpdateAsync(user);
				if (!updateResult.Succeeded)
				{
					throw new AppValidationException(updateResult.Errors.Select(e => e.Description));
				}
			}

			// 7. Commit changes
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new EmployeeDto
			{
				Id = employee.Id,
				FullName = employee.FullName,
				Email = employee.Email,
				PhoneNumber = employee.PhoneNumber,
				Role = employee.Role,
				Status = employee.Status,
				CreatedAt = employee.CreatedAt,
				UpdatedAt = employee.UpdatedAt
			};

		}

		public async Task<EmployeeDto> UpdateStatusAsync(Guid id, UpdateEmployeeStatusDto request, CancellationToken cancellationToken)
		{
			// 1. Fluent Validation
			var validationResult = await _statusValidator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}
			// 2. Fetch Employee within active tenant scope
			var employee = await _employeesRepository.GetEntityByIdAsync(id, cancellationToken);

			if (employee == null)
			{
				throw new NotFoundException("Employee not found.");
			}

			var user = employee.User;

			// 3. Update status
			employee.Status = request.Status;
			employee.UpdatedAt = DateTime.UtcNow;

			if (user != null)
			{
				user.Status = request.Status;
				user.UpdatedAt = DateTime.UtcNow;
				var updateResult = await _userManager.UpdateAsync(user);
				if (!updateResult.Succeeded)
				{
					throw new AppValidationException(updateResult.Errors.Select(e => e.Description));
				}
			}

			// 4. Commit changes
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new EmployeeDto
			{
				Id = employee.Id,
				FullName = employee.FullName,
				Email = employee.Email,
				PhoneNumber = employee.PhoneNumber,
				Role = employee.Role,
				Status = employee.Status,
				CreatedAt = employee.CreatedAt,
				UpdatedAt = employee.UpdatedAt
			};
		}
	}
}
