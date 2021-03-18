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
		public const int DefaultErrorStatusCode = StatusCodes.Status500InternalServerError;
		public const string DefaultErrorMessage = "Server error occured";
		public const string DefaultErrorContentType = "application/json";

		public static async Task HandleGlobalExceptionAsync(HttpContext context)
		{
			var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
			var exception = exceptionHandlerPathFeature.Error;

			var errorId = Guid.NewGuid();
			var statusCode = DefaultErrorStatusCode;
			var errorMessage = DefaultErrorMessage;

			if (exception is UserFriendlyException userFriendlyException)
			{
				statusCode = userFriendlyException.StatusCode ?? statusCode;
				errorMessage = userFriendlyException.Message;
			}

			var serviceError = new ServiceError
			{
				ErrorId = errorId,
				ErrorMessage = errorMessage,
			};
			var serviceErrorJson = JsonSerializer.Serialize(serviceError);

			context.Response.StatusCode = statusCode;
			context.Response.ContentType = DefaultErrorContentType;
			await context.Response.WriteAsync(serviceErrorJson);

			var logger = context.RequestServices.GetService<ILoggingManager>();
			logger?.Error(serviceErrorJson, exception);
		}
	}
}
