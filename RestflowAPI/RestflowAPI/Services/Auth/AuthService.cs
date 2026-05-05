using Microsoft.AspNetCore.Identity.Data;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.ServiceInterfaces.Auth;

namespace RestflowAPI.Services.Auth
{
	public class AuthService : IAuthService
	{
		public Task<AuthResponseDto> RegisterAsync(RegisterRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
