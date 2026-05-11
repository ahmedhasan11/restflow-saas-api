using RestflowAPI.ServiceInterfaces.Auth;
using System.Security.Cryptography;

namespace RestflowAPI.Services.Auth
{
	public class RefreshTokenService : IRefreshTokenService
	{
		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[64];
			using var rng= RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
	}
}
