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

			return HandleServiceResult(controller, serviceResult.Success, serviceResult.Content,
				serviceResult.ErrorMessage);
		}

		public static ActionResult Service(this ControllerBase controller, ServiceResult serviceResult)
		{
			if (serviceResult == null)
				throw new ArgumentNullException(nameof(serviceResult), "Service result can't be null");

			return HandleServiceResult(controller, serviceResult.Success, errorMsg: serviceResult.ErrorMessage);
		}

		private static ActionResult HandleServiceResult(ControllerBase controller, bool success, object content = null,
			string errorMsg = null)
			=> success
				? content == null
					? controller.Ok()
					: controller.Ok(content)
				: controller.BadRequest(errorMsg);
	}
}
