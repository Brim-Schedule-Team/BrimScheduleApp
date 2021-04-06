using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Hosting;

namespace BrimSchedule.API.Utility
{
	public class ExposeInEnvironmentControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
	{
		private readonly IHostEnvironment _hostEnvironment;

		public ExposeInEnvironmentControllerFeatureProvider(IHostEnvironment hostEnvironment)
		{
			_hostEnvironment = hostEnvironment;
		}

		public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
		{
			foreach (var controllerType in feature.Controllers.ToArray())
			{
				var exposeInEnvironmentAttribute = controllerType.GetCustomAttribute<ExposeInEnvironmentAttribute>();
				if (exposeInEnvironmentAttribute != null && exposeInEnvironmentAttribute.Environment != _hostEnvironment.EnvironmentName)
					feature.Controllers.Remove(controllerType);
			}
		}
	}
}
