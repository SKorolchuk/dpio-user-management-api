using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Deeproxio.Infrastructure.Runtime
{
	public class SelfHostServerBuilder<T> : IDisposable where T : class, IServerStartup
	{
		private readonly string[] _args;
		private readonly int _defaultPort;
		private IWebHost _serverConfiguration;

		public SelfHostServerBuilder(string[] args, int defaultPort = 5001)
		{
			_args = args;
			_defaultPort = defaultPort;
		}

		public IWebHost Build()
		{
			var initialSetup = WebHost.CreateDefaultBuilder(_args)
				.UseLibuv();

			if (Environment.GetEnvironmentVariable("ENVIRONMENT") != "Production")
			{
				initialSetup = initialSetup.UseKestrel(options =>
				{
					options.Listen(IPAddress.Loopback, !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PORT"))
							? int.Parse(Environment.GetEnvironmentVariable("PORT"))
							: _defaultPort);
				});
			}
			else
			{
				initialSetup = initialSetup.UseKestrel();
			}

			_serverConfiguration = initialSetup
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<T>()
				.Build();

			return _serverConfiguration;
		}

		public void Dispose()
		{
			if (_serverConfiguration == null)
				return;

			_serverConfiguration.Dispose();
			_serverConfiguration = null;
		}
	}
}
