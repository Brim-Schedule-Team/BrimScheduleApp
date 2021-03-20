using System;
using System.Threading.Tasks;
using BrimSchedule.API.Utility;
using BrimSchedule.Application.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BrimSchedule.API.Services
{
	public static class ExceptionHandler
	{
		public const int DefaultErrorStatusCode = StatusCodes.Status500InternalServerError;
		public const string DefaultErrorMessage = "Server error occured";
		public const string DefaultErrorContentType = "application/json";

		public static async Task HandleGlobalExceptionAsync(HttpContext context, bool isDevelopment = false)
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

			var endpointError = new EndpointError
			{
				ErrorId = errorId,
				ErrorMessage = errorMessage,
			};

			var responseContent = SerializeEndpointError(endpointError, exception, isDevelopment);

			context.Response.StatusCode = statusCode;
			context.Response.ContentType = DefaultErrorContentType;
			await context.Response.WriteAsync(responseContent);

			var logger = context.RequestServices.GetService<ILoggingManager>();
			logger?.Error(responseContent, exception);
		}

		private static string SerializeEndpointError(EndpointError endpointError, Exception originalException, bool isDevelopment)
		{
			// Include original exception and beautify Json response for Development mode
			if (isDevelopment)
			{
				var serializerSettings = new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore,
				};
				endpointError.OriginalException = originalException;
				return JsonConvert.SerializeObject(endpointError, serializerSettings);
			}

			// Use System.Text.Json for Production serialization
			return JsonSerializer.Serialize(endpointError);
		}
	}
}
