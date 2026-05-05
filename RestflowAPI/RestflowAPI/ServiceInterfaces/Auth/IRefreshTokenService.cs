using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.ServiceInterfaces.Auth
{
	public interface IRefreshTokenService
	{
		string GenerateRefreshToken();
	}
}
