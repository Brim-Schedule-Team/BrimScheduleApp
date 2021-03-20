using System;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Utility
{
	public static class ControllerExtensions
	{
		public static ActionResult Service(this Controller controller, ServiceResult serviceResult)
		{
			if (serviceResult == null)
				throw new ArgumentNullException(nameof(serviceResult), "You can't return null service result");

			return serviceResult.IsSuccess
				? controller.Ok(serviceResult.Content)
				: controller.BadRequest(serviceResult.ErrorMessage);
		}
	}
}
