using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;
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

		public async Task SaveOtpAsync(OtpVerification otp, CancellationToken cancellationToken)
		{
			await _context.Set<OtpVerification>().AddAsync(otp, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
