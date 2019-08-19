using System.Threading.Tasks;

namespace Deeproxio.Infrastructure.Notification
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string message);
	}
}