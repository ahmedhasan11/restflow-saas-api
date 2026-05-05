using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.ServiceInterfaces.Auth
{
	public interface IJwtService
	{
		Task<JwtResultDto> GenerateTokenAsync(JwtUserDataDto userData);
	}
}
