using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestflowAPI.Entities;
using RestflowAPI.ServiceInterfaces.Tenants;
using System.Reflection;

namespace RestflowAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICurrentTenantService _tenantService;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InventoryCategory> InventoryCategories => Set<InventoryCategory>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductIngredient> ProductIngredients => Set<ProductIngredient>();
    public DbSet<PlatformSetting> PlatformSettings => Set<PlatformSetting>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		// Override query filters for multi-tenant entities to enforce tenant isolation
		modelBuilder.Entity<Customer>().HasQueryFilter(e => e.DeletedAt == null && e.TenantId == _tenantService.TenantId);
		modelBuilder.Entity<InventoryItem>().HasQueryFilter(e => e.DeletedAt == null && e.TenantId == _tenantService.TenantId);
		modelBuilder.Entity<MenuCategory>().HasQueryFilter(e => e.DeletedAt == null && e.TenantId == _tenantService.TenantId);
		modelBuilder.Entity<Product>().HasQueryFilter(e => e.DeletedAt == null && e.TenantId == _tenantService.TenantId);
		modelBuilder.Entity<ProductIngredient>().HasQueryFilter(e => e.DeletedAt == null && e.TenantId == _tenantService.TenantId);
		modelBuilder.Entity<StockMovement>().HasQueryFilter(e => e.DeletedAt == null && e.TenantId == _tenantService.TenantId);
		modelBuilder.Entity<ApplicationUser>().HasQueryFilter(e => e.DeletedAt == null && (e.TenantId == _tenantService.TenantId || e.TenantId == null));
	}
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var now = DateTime.UtcNow;

		foreach (var entry in ChangeTracker.Entries<IAuditable>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedAt = now;
					// Auto-inject TenantId if applicable
					if (entry.Entity is IMustHaveTenant tenantEntity && tenantEntity.TenantId == Guid.Empty && _tenantService.TenantId.HasValue)
					{
						tenantEntity.TenantId = _tenantService.TenantId.Value;
					}             
					// For ApplicationUser, which might have a null TenantId (Super Admin)
					if (entry.Entity is ApplicationUser user && user.TenantId == null && _tenantService.TenantId.HasValue)
					{
						user.TenantId = _tenantService.TenantId.Value;
					}
					break;

				case EntityState.Modified:
					entry.Entity.UpdatedAt = now;
					break;

				case EntityState.Deleted:
					// Soft delete logic
					entry.State = EntityState.Modified;
					entry.Entity.DeletedAt = now;
					entry.Entity.UpdatedAt = now;
					break;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}
