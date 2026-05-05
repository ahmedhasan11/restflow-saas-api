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
			return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
		}

		public async Task<ApplicationUser?> FindByPhoneAsync(string phone, CancellationToken cancellationToken)
		{
			return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone, cancellationToken);
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
				.FirstOrDefaultAsync(u => (u.Email == identifier || u.PhoneNumber == identifier), cancellationToken);
		}
	}
}
