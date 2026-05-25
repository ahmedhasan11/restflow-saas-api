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

		public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto request, CancellationToken cancellationToken)
		{
			// 1. Fluent Validation
			var validationResult = await _updateEmployeeValidator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			// 2. Fetch user within the owner's active tenant scope (standard query filter is fine)
			var tenantId = _tenantService.TenantId;
			var user = await _userManager.Users
				.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId, cancellationToken);

			if (user == null)
			{
				throw new NotFoundException("Employee not found.");
			}

			// 3. Unique Email Check (if changed)
			if (request.Email != null && !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
			{
				var existingEmail = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
				if (existingEmail != null)
				{
					throw new ConflictException("Email is already in use.");
				}
				user.Email = request.Email;
				user.UserName = request.Email;
				user.NormalizedEmail = request.Email.ToUpper();
				user.NormalizedUserName = request.Email.ToUpper();
			}
			// 4. Unique Phone Check (if changed)
			if (request.PhoneNumber != null && user.PhoneNumber != request.PhoneNumber)
			{
				var existingPhone = await _authRepository.FindByPhoneAsync(request.PhoneNumber, cancellationToken);
				if (existingPhone != null)
				{
					throw new ConflictException("Phone number is already in use.");
				}
				user.PhoneNumber = request.PhoneNumber;
			}
			// 5. Update other fields
			if (request.FullName != null)
			{
				user.FullName = request.FullName;
			}

			if (request.Status != null)
			{
				user.Status = request.Status.Value;
			}
			string currentRole = request.Role ?? string.Empty;
			if (request.Role != null)
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
			else
			{
				var currentRoles = await _userManager.GetRolesAsync(user);
				currentRole = currentRoles.FirstOrDefault() ?? string.Empty;
			}

			user.UpdatedAt = DateTime.UtcNow;

			var updateResult = await _userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				throw new AppValidationException(updateResult.Errors.Select(e => e.Description));
			}

			// 7. Commit changes
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new EmployeeDto
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email ?? string.Empty,
				PhoneNumber = user.PhoneNumber ?? string.Empty,
				Role = currentRole,
				Status = user.Status,
				CreatedAt = user.CreatedAt,
				UpdatedAt = user.UpdatedAt
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
			// 2. Fetch user within the active tenant scope (standard filter is fine)
			var tenantId = _tenantService.TenantId;
			var user = await _userManager.Users
				.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId, cancellationToken);

			if (user == null)
			{
				throw new NotFoundException("Employee not found.");
			}

			// 3. Update status
			user.Status = request.Status;
			user.UpdatedAt = DateTime.UtcNow;

			var updateResult = await _userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				throw new AppValidationException(updateResult.Errors.Select(e => e.Description));
			}

			// 4. Commit changes
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// 5. Get user roles
			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.FirstOrDefault() ?? string.Empty;

			return new EmployeeDto
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email ?? string.Empty,
				PhoneNumber = user.PhoneNumber ?? string.Empty,
				Role = role,
				Status = user.Status,
				CreatedAt = user.CreatedAt,
				UpdatedAt = user.UpdatedAt
			};
		}
	}
}
