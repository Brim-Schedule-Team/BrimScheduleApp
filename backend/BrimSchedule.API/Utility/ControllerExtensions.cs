using System;
using BrimSchedule.Application;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Utility
{
	public static class ControllerExtensions
	{
		public static ActionResult Service<T>(this Controller controller, ServiceResult<T> serviceResult)
		{
			if (serviceResult == null)
				throw new ArgumentNullException(nameof(serviceResult), "Service result can't be null");

			return serviceResult.Success
				? controller.Ok(serviceResult.Content)
				: controller.BadRequest(serviceResult.ErrorMessage);
		}
	}
}
