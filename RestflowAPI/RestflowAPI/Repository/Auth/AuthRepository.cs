using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.RepositoryInterfaces.Auth;
using System.Threading;

namespace RestflowAPI.Repository.Auth
{
	public class AuthRepository : IAuthRepository
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public AuthRepository(
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			ApplicationDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}
		public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
		{
			return await _userManager.AddToRoleAsync(user, role);
		}

		public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
		{
			return await _userManager.CheckPasswordAsync(user, password);
		}

		public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
		{
			return await _userManager.CreateAsync(user, password);
		}

		public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken)
		{
			return await _userManager.Users
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(u => u.DeletedAt == null && (u.Email == email || u.NormalizedEmail == email.ToUpper()), cancellationToken);
		}

		public async Task<ApplicationUser?> FindByPhoneAsync(string phone, CancellationToken cancellationToken)
		{
			return await _userManager.Users
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(u => u.DeletedAt == null && u.PhoneNumber == phone, cancellationToken);
		}

		public async Task<OtpVerification?> GetLatestOtpAsync(Guid userId, ChannelType channel, CancellationToken cancellationToken)
		{
			return await _context.Set<OtpVerification>()
				.Where(otp => otp.UserId == userId && otp.ChannelType == channel)
				.OrderByDescending(otp => otp.CreatedAt)
				.FirstOrDefaultAsync(cancellationToken);
		}


		public async Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user)
		{
			return await _userManager.GetRolesAsync(user);
		}

		public async Task InvalidateOldOtpsAsync(Guid userId, ChannelType channel, CancellationToken cancellationToken)
		{
			var oldOtps = await _context.Set<OtpVerification>()
				.Where(o => o.UserId == userId && o.ChannelType == channel && !o.IsUsed)
				.ToListAsync(cancellationToken);

			foreach (var otp in oldOtps)
			{
				otp.ExpiresAt = DateTime.UtcNow;
			}
		}

		public async Task SaveOtpAsync(OtpVerification otp, CancellationToken cancellationToken)
		{
			await _context.Set<OtpVerification>().AddAsync(otp, cancellationToken);
		}
		public async Task<ApplicationUser?> FindByIdentifierAsync(string identifier, CancellationToken cancellationToken)
		{
			return await _userManager.Users
				.Include(u => u.Tenant)
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(u => u.DeletedAt == null && (u.Email == identifier || u.NormalizedEmail == identifier.ToUpper() || u.PhoneNumber == identifier), cancellationToken);
		}

		public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
		{
			await _userManager.RemovePasswordAsync(user);
			return await _userManager.AddPasswordAsync(user, newPassword);
		}

		public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
		{
			var tokens = await _context.Set<RefreshToken>()
				.Where(r => r.UserId == userId && !r.IsRevoked)
				.ToListAsync(cancellationToken);

			foreach (var token in tokens)
			{
				token.IsRevoked = true;
			}
		}

		public async Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
		{
			return await _userManager.Users
				.Include(u => u.Tenant)
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(u => u.DeletedAt == null && u.Id == userId, cancellationToken);
		}

		public async Task<IdentityResult> IncrementAccessFailedCountAsync(ApplicationUser user)
		{
			return await _userManager.AccessFailedAsync(user);
		}

		public async Task<IdentityResult> ResetAccessFailedCountAsync(ApplicationUser user)
		{
			return await _userManager.ResetAccessFailedCountAsync(user);
		}

		public async Task<bool> IsLockedOutAsync(ApplicationUser user)
		{
			return await _userManager.IsLockedOutAsync(user);
		}

		public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
		{
			return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
		}
	}
}
