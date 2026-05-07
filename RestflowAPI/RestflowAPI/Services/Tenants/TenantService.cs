using FluentValidation;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Tenants;
using RestflowAPI.Entities;
using RestflowAPI.Exceptions;
using RestflowAPI.RepositoryInterfaces.Tenants;
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
				CreatedAt = t.CreatedAt
			});
		}
	}
}
