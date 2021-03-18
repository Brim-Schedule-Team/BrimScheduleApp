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
			var logger = InitializeLogger(host);
			logger.Info("Starting web host");

			try
			{
				ExecuteDatabaseMigrations(host);
				host.Run();
			}
			catch (Exception ex)
			{
				logger.Error("Application encountered a critical error and will be shut down", ex);
				throw;
			}
		}

		private static ILoggingManager InitializeLogger(IHost host)
		{
			using var scope = host.Services.CreateScope();
			var logger = scope.ServiceProvider.GetRequiredService<ILoggingManager>();
			return logger;
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
