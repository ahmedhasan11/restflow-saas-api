using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.Settings;

namespace RestflowAPI.Services.Auth
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}
		public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subject;

			var bodyBuilder = new BodyBuilder { TextBody = isHtml ? null : body, HtmlBody = isHtml ? body : null };
			message.Body = bodyBuilder.ToMessageBody();

			using var client = new SmtpClient();
			try
			{
				await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort,
					_emailSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

				await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
			}
			catch (Exception ex)
			{
				// Log the exception if needed, but here we'll just rethrow for the middleware to catch
				throw new Exception("Failed to send email. Please check SMTP settings.", ex);
			}
		}
	}
}
