using System.Net;

namespace RestflowAPI.Exceptions
{
	public abstract class BaseException : Exception
	{
		public HttpStatusCode StatusCode { get; }
		public IEnumerable<string>? Errors { get; }

		protected BaseException(string message, HttpStatusCode statusCode, IEnumerable<string>? errors = null)
			: base(message)
		{
			StatusCode = statusCode;
			Errors = errors;
		}
	}
}
