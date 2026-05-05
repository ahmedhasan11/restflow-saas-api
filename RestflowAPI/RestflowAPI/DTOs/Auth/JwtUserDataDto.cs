namespace RestflowAPI.DTOs.Auth
{
	public class JwtUserDataDto
	{
		public Guid UserId { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public Guid? TenantId { get; set; }
		public IEnumerable<string> Roles { get; set; } = new List<string>();
	}
}
