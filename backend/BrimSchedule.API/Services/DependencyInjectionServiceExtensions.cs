using BrimSchedule.Application.Logging;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;
using BrimSchedule.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrimSchedule.API.Services
{
	public static class ServiceExtensions
	{
		public static void ConfigureDependencyInjection(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
		{
			InjectCommonServices(services, configuration);

			if (isDevelopment)
			{
				InjectServicesForDevelopmentEnvironment(services, configuration);
			}
			else
			{
				InjectServicesForProductionEnvironment(services, configuration);
			}
		}

		/// <summary>
		/// Set up DI for services specific for any environment
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		private static void InjectCommonServices(IServiceCollection services, IConfiguration configuration)
		{
			// Inject DB context and repositories
			var connectionString = configuration.GetConnectionString(nameof(BrimScheduleContext));
			services.AddDbContext<BrimScheduleContext>(opts => opts.UseNpgsql(connectionString));
			services.AddScoped<IUnitOfWork, EFUnitOfWork>(provider =>
				new EFUnitOfWork(provider.GetService<BrimScheduleContext>()));

			// Inject logger singleton
			services.AddSingleton<ILoggingManager, LoggingManager>();
		}

		/// <summary>
		/// Set up DI for services specific for Development environment
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		private static void InjectServicesForDevelopmentEnvironment(IServiceCollection services, IConfiguration configuration)
		{
		}

		/// <summary>
		/// Set up DI for services specific for Production environment
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		private static void InjectServicesForProductionEnvironment(IServiceCollection services, IConfiguration configuration)
		{
		}
	}
}
