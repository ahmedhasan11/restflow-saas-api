using System.Net;

namespace RestflowAPI.Exceptions
{
	public class AppValidationException : BaseException
	{
		public AppValidationException(string message)
			: base(message, HttpStatusCode.BadRequest)
		{
		}

		public AppValidationException(IEnumerable<string> errors)
			: base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, errors)
		{
		}
	}
}
