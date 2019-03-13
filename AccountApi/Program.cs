using Deeproxio.Infrastructure.Runtime;
using Microsoft.AspNetCore.Hosting;

namespace Deeproxio.AccountApi
{
    public class Program
    {
		public static void Main(string[] args)
        {
	        using (var server = new SelfHostServerBuilder<Startup>(args, 8080))
			{
		        server.Build().Run();
	        }
        }
    }
}
