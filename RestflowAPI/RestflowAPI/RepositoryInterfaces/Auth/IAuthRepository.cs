using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.RepositoryInterfaces.Auth
{
	public interface IAuthRepository
	{
		Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken);
		Task<ApplicationUser?> FindByPhoneAsync(string phone, CancellationToken cancellationToken);
		Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
		Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
		Task SaveOtpAsync(OtpVerification otp, CancellationToken cancellationToken);

		Task<ApplicationUser?> FindByIdentifierAsync(string identifier, CancellationToken cancellationToken);
		Task InvalidateOldOtpsAsync(Guid userId, ChannelType channel, CancellationToken cancellationToken);
		// Future: Task<OtpVerification?> GetValidOtpAsync(Guid userId, ChannelType channel, string codeHash);
		// New methods for verification
		Task<OtpVerification?> GetLatestOtpAsync(Guid userId, ChannelType channel, CancellationToken cancellationToken);
		Task<bool> CheckPasswordAsync(ApplicationUser user , string password);
		Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user);
		Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword);

		Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);

		Task<IdentityResult> IncrementAccessFailedCountAsync(ApplicationUser user);
		Task<IdentityResult> ResetAccessFailedCountAsync(ApplicationUser user);
		Task<bool> IsLockedOutAsync(ApplicationUser user);
	}
}

