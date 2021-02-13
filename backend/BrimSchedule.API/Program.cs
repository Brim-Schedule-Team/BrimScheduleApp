using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BrimSchedule.API
{
	#pragma warning disable CA1052
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseDefaultServiceProvider(options => options.ValidateScopes = false) // for EF core
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}
}
