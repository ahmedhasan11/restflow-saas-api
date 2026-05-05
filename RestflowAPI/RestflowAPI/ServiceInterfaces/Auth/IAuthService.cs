using Microsoft.AspNetCore.Identity.Data;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.ServiceInterfaces.Auth
{
	public interface IAuthService
	{
		Task<AuthResponseDto> RegisterAsync(RegisterRequest request);
	}
}
