using Microsoft.EntityFrameworkCore;
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

		public async Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken)
		{
			return await _db.Set<RefreshToken>().Include(r=>r.User).ThenInclude(u=>u.Tenant).FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
		}

		public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
		{
			var tokens = await _db.Set<RefreshToken>()
				.Where(r => r.UserId == userId && !r.IsRevoked)
				.ToListAsync(cancellationToken);

			foreach (var token in tokens)
			{
				token.IsRevoked = true;
			}
		}

		public async Task SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
		{
			await _db.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
		}
	}
}
