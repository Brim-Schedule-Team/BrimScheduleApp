using System;
using BrimSchedule.Application.Logging;
using BrimSchedule.Persistence.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BrimSchedule.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			InitializeLoggingSystem(host);
			ExecuteDatabaseMigrations(host);

			host.Run();
		}

		private static void InitializeLoggingSystem(IHost host)
		{
			using var scope = host.Services.CreateScope();
			var logger = scope.ServiceProvider.GetRequiredService<ILoggingManager>();
			logger.Info("Application started");
		}

		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

		private static void ExecuteDatabaseMigrations(IHost host)
		{
			using var scope = host.Services.CreateScope();
			using var context = scope.ServiceProvider.GetRequiredService<BrimScheduleContext>();
			context.Database.Migrate();
		}
	}
}
