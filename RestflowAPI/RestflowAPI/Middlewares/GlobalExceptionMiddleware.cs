using System.Net;
using System.Text.Json;

namespace RestflowAPI.Middlewares
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

			// Default to 500 Internal Server Error
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var response = new ErrorResponse
			{
				StatusCode = context.Response.StatusCode,
				Message = "An internal server error occurred. Please try again later.",
				DetailedMessage = _env.IsDevelopment() ? exception.Message : null,
				StackTrace = _env.IsDevelopment() ? exception.StackTrace : null
			};

			// You can add logic here to handle specific exception types (e.g., UnauthorizedAccessException, etc.)
			// For now, it handles everything as a general error.

			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			var json = JsonSerializer.Serialize(response, options);

			await context.Response.WriteAsync(json);
		}
	}

	public class ErrorResponse
	{
		public int StatusCode { get; set; }
		public string Message { get; set; } = string.Empty;
		public string? DetailedMessage { get; set; }
		public string? StackTrace { get; set; }
	}
}
