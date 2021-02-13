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
			var connectionString = configuration.GetConnectionString(nameof(BrimScheduleContext));
			// EFUnitOfWork is the main DB class to work with in the Business layer
			services.AddScoped<IUnitOfWork>(_ => new EFUnitOfWork(connectionString));
			// Register BrimScheduleContext for auto migrations
			services.AddDbContext<BrimScheduleContext>(opts => opts.UseNpgsql(connectionString));
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
