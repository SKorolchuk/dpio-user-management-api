using System.Threading.Tasks;

namespace Deeproxio.Infrastructure.Notification
{
	public class AuthMessageSender: IEmailSender, ISmsSender
	{
		public static Task SendEmailAsync(string email, string subject, string message)
		{
			// Plug in your email service
			return Task.FromResult(0);
		}

		public static Task SendSmsAsync(string number, string message)
		{
			// Plug in your sms service
			return Task.FromResult(0);
		}

		Task IEmailSender.SendEmailAsync(string email, string subject, string message)
		{
			return SendEmailAsync(email, subject, message);
		}

		Task ISmsSender.SendSmsAsync(string number, string message)
		{
			return SendSmsAsync(number, message);
		}
	}
}