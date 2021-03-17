using System;
using System.Text.Json;
using System.Threading.Tasks;
using BrimSchedule.API.Models;
using BrimSchedule.Application.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BrimSchedule.API.Services
{
	public static class ExceptionHandler
	{
		public static async Task HandleGlobalExceptions(HttpContext context)
		{
			context.Response.StatusCode = 500;
			context.Response.ContentType = "application/json";

			var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
			var exception = exceptionHandlerPathFeature.Error;

			const string defaultErrorMessage = "Server error occured";

			var errorMessage = exception is UserFriendlyException ? exception.Message : defaultErrorMessage;
			var errorId = Guid.NewGuid();
			var serviceError = new ServiceError
			{
				ErrorId = errorId,
				ErrorMessage = errorMessage,
			};

			var serviceErrorJson = JsonSerializer.Serialize(serviceError);
			await context.Response.WriteAsync(serviceErrorJson);

			var logger = context.RequestServices.GetService<ILoggingManager>();
			logger?.Error(serviceErrorJson, exception);
		}
	}
}
