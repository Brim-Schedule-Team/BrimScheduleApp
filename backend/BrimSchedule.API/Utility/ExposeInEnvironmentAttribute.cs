using System;
using Microsoft.Extensions.Hosting;

namespace BrimSchedule.API.Utility
{
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class ExposeInEnvironmentAttribute : Attribute
	{
		public string Environment { get; }

		protected ExposeInEnvironmentAttribute(string environment)
		{
			Environment = environment;
		}
	}

	public class DevOnlyAttribute : ExposeInEnvironmentAttribute
	{
		public DevOnlyAttribute() : base(Environments.Development)
		{
		}
	}
}
