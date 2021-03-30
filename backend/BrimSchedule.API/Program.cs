using System;
using BrimSchedule.API.Config;
using BrimSchedule.API.Services;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Application.Logging;
using BrimSchedule.Persistence.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
				Seed(host);
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
			return scope.ServiceProvider.GetRequiredService<ILoggingManager>();
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

		private static void Seed(IHost host)
		{
			using var scope = host.Services.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var seederOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeederOptions>>();
			new Seeder(userService, seederOptions).Seed().Wait();
		}
	}
}
