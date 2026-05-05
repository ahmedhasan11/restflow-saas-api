using RestflowAPI.Entities;

namespace RestflowAPI.RepositoryInterfaces.Auth
{
	public interface IRefreshTokenRepository
	{
		Task SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
		Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken);
	}
}
