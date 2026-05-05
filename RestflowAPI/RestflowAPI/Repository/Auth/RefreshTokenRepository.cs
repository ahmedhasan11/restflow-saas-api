using RestflowAPI.Data;
using RestflowAPI.Entities;
using RestflowAPI.RepositoryInterfaces.Auth;

namespace RestflowAPI.Repository.Auth
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly ApplicationDbContext _db;
		public RefreshTokenRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
		{
			await _db.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
		}
	}
}
