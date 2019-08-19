using System.Threading.Tasks;

namespace Deeproxio.Infrastructure.Notification
{
	public interface ISmsSender
	{
		Task SendSmsAsync(string number, string message);
	}
}
