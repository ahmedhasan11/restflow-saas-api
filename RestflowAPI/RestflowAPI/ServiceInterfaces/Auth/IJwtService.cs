using RestflowAPI.DTOs.Auth;
using System.Security.Claims;

namespace RestflowAPI.ServiceInterfaces.Auth
{
	public interface IJwtService
	{
		Task<JwtResultDto> GenerateTokenAsync(JwtUserDataDto userData);
		ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
	}
}
