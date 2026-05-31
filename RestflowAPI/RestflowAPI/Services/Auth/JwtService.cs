using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestflowAPI.Services.Auth
{
	public class JwtService:IJwtService
	{
		private readonly JwtSettings _jwtSettings;
		public JwtService(IOptions<JwtSettings> jwtSettings)
		{
			_jwtSettings = jwtSettings.Value;
		}
		 
		public async Task<JwtResultDto> GenerateTokenAsync(JwtUserDataDto userData)
		{
			var claimsjwt = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, userData.UserId.ToString()),
			new Claim(ClaimTypes.NameIdentifier, userData.UserId.ToString()),
			new Claim(JwtRegisteredClaimNames.Email, userData.Email),
			new Claim(ClaimTypes.Email, userData.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Name, userData.FullName),
			new Claim(ClaimTypes.Name, userData.FullName),
			new Claim("TenantId", userData.TenantId?.ToString() ?? "")
		};

			foreach (var role in userData.Roles)
			{
				claimsjwt.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

			var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer, audience: _jwtSettings.Audience
				, claims: claimsjwt, expires: expiration, signingCredentials: creds);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
			return new JwtResultDto { Token = tokenString };
		}
		public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
				ValidateLifetime = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");

			return principal;
		}
	}
}
