using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Auth
{
	public interface IRefreshTokenRepository
	{
		Task SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
		Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken);

		Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken);
	}
}
