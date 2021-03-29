using System;
using BrimSchedule.Application;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Utility
{
	public static class ControllerExtensions
	{
		public static ActionResult Service<TModel>(this ControllerBase controller, ServiceResult<TModel> serviceResult)
		{
			if (serviceResult == null)
				throw new ArgumentNullException(nameof(serviceResult), "Service result can't be null");

			return serviceResult.Success
				? controller.Ok(serviceResult.Content)
				: controller.BadRequest(serviceResult.ErrorMessage);
		}

		public static ActionResult Service(this ControllerBase controller, ServiceResult serviceResult)
		{
			if (serviceResult == null)
				throw new ArgumentNullException(nameof(serviceResult), "Service result can't be null");

			return serviceResult.Success
				? controller.Ok()
				: controller.BadRequest(serviceResult.ErrorMessage);
		}
	}
}
