using System.Net;

namespace RestflowAPI.Exceptions
{
	public class UnauthorizedException : BaseException
	{
		public UnauthorizedException(string message)
			: base(message, HttpStatusCode.Unauthorized)
		{
		}
	}
}
