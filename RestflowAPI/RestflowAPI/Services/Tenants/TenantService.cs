using FluentValidation;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Tenants;
using RestflowAPI.Entities;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.Tenants;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.Tenants
{
	public class TenantService : ITenantService
	{
		private readonly ITenantRepository _tenantRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateTenantRequestDto> _createTenantValidator;

		public TenantService(ITenantRepository tenantRepository, IUnitOfWork unitOfWork, IValidator<CreateTenantRequestDto> createTenantValidator)
		{
			_tenantRepository = tenantRepository;
			_unitOfWork = unitOfWork;
			_createTenantValidator = createTenantValidator;	
		}

		public async Task<TenantResponseDto> ChangeTenantStatusAsync(Guid tenantId, ChangeTenantStatusDto request, CancellationToken cancellationToken)
		{
			var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
			if (tenant == null)
			{
				throw new Exceptions.NotFoundException("Tenant not found.");
			}

			tenant.Status = request.Status;
			tenant.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new TenantResponseDto
			{
				Id = tenant.Id,
				RestaurantName = tenant.RestaurantName,
				TenantCode = tenant.TenantCode,
				Status = tenant.Status,
				Country = tenant.Country,
				DefaultLanguage = tenant.DefaultLanguage,
				Timezone = tenant.Timezone,
				Currency = tenant.Currency,
				CreatedAt = tenant.CreatedAt
			};
		}

		public async Task<TenantResponseDto> CreateTenantAsync(CreateTenantRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _createTenantValidator.ValidateAsync(request, cancellationToken);
			if(!result.IsValid)
			{
				throw new ValidationException(result.Errors);     //Custom Exception
			}
			var existingTenant = await _tenantRepository.GetByCodeAsync(request.TenantCode, cancellationToken);

			if(existingTenant != null)
			{
				throw new ConflictException($"Tenant with code '{request.TenantCode}' already exists.");
			}

			Tenant tenant = new Tenant
			{
				TenantCode = request.TenantCode,
				RestaurantName = request.RestaurantName,
				Status = request.Status,
				Country = request.Country,
				DefaultLanguage = request.DefaultLanguage,
				Timezone = request.Timezone,
				Currency = request.Currency,
				CreatedAt = DateTime.UtcNow
			};

			await _tenantRepository.CreateAsync(tenant, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return new TenantResponseDto
			{
				Id = tenant.Id,
				TenantCode = tenant.TenantCode,
				RestaurantName = tenant.RestaurantName,
				Status = tenant.Status,
				Country = tenant.Country,
				DefaultLanguage = tenant.DefaultLanguage,
				Timezone = tenant.Timezone,
				Currency = tenant.Currency,
				CreatedAt = tenant.CreatedAt
			};
		}

		public async Task<IEnumerable<TenantResponseDto>> GetAllTenantsAsync(CancellationToken cancellationToken)
		{
		    var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
			return tenants.Select(t => new TenantResponseDto
			{
				Id = t.Id,
				RestaurantName = t.RestaurantName,
				TenantCode = t.TenantCode,
				Status = t.Status,
				Country = t.Country,
				DefaultLanguage = t.DefaultLanguage,
				Timezone = t.Timezone,
				Currency = t.Currency,
				CreatedAt = t.CreatedAt
			});
		}
	}
}
