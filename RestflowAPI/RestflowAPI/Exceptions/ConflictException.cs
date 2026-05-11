using System.Net;

namespace RestflowAPI.Exceptions
{
	public class ConflictException : BaseException
	{
		public ConflictException(string message)
			: base(message, HttpStatusCode.Conflict)
		{
		}
	}
}
