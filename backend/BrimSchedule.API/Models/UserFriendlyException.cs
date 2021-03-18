using System;
using Microsoft.AspNetCore.Http;

namespace BrimSchedule.API.Models
{
	// Throw this exception from controller if you want to return user a meaningful error message
	public class UserFriendlyException: Exception
	{
		public int? StatusCode { get; set; }

		public UserFriendlyException()
		{
		}

		public UserFriendlyException(string message): base(message)
		{
		}

		public UserFriendlyException(string message, Exception innerException): base(message, innerException)
		{
		}

		public UserFriendlyException(string message, Exception innerException, int statusCode) : this(message, innerException)
		{
			StatusCode = statusCode;
		}
	}
}
