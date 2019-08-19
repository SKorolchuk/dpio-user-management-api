using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deeproxio.Domain.Models;
using Deeproxio.Persistence.Context;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("Wait...");

	        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
		        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

	        IConfigurationRoot configuration = builder.Build();	   

	        var connectionString = configuration.GetConnectionString(nameof(DomainContext));

	        List<News> news = new List<News>();

			using (var ctx = new DomainContext(connectionString))
			{
				news = ctx.News.ToList();
			}

	        foreach (var newsItem in news)
	        {
		        Console.WriteLine($"Name: {newsItem.Name} Desc: {newsItem.Description} Created: {newsItem.CreateTS}");
	        }

			Console.WriteLine(connectionString);

	        Console.ReadKey();
        }
    }
}
