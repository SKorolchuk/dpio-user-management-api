using System.Linq;
using Deeproxio.Infrastructure.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Deeproxio.AccountApi
{
    public class Program
    {
		public static void Main(string[] args)
        {
            var seed = args.Any(x => x == "/seed");
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
            }

	        using (var server = new SelfHostServerBuilder<Startup>(args, 8080))
            {
                var host = server.Build();
                if (seed)
                {
                    using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        SeedIDForDatabase.EnsureSeedData(scope.ServiceProvider);
                        return;
                    }
                }

                host.Run();
	        }
        }
    }
}
