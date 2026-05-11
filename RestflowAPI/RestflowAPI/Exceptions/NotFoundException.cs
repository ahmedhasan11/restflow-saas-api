using System.Net;

namespace RestflowAPI.Exceptions
{
	public class NotFoundException : BaseException
	{
		public NotFoundException(string message)
			: base(message, HttpStatusCode.NotFound)
		{
		}

		public NotFoundException(string name, object key)
			: base($"Entity \"{name}\" ({key}) was not found.", HttpStatusCode.NotFound)
		{
		}
	}
}
