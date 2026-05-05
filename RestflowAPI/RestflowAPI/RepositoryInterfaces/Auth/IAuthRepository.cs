using Microsoft.AspNetCore.Identity;
using RestflowAPI.Entities;

namespace RestflowAPI.RepositoryInterfaces.Auth
{
	public interface IAuthRepository
	{
		Task<ApplicationUser?> FindByEmailAsync(string email);
		Task<ApplicationUser?> FindByPhoneAsync(string phone);
		Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
		Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
		Task SaveOtpAsync(OtpVerification otp);

		// Future: Task<OtpVerification?> GetValidOtpAsync(Guid userId, ChannelType channel, string codeHash);
	}
}
