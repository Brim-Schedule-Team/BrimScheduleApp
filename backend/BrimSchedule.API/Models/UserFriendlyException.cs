using System;

namespace BrimSchedule.API.Models
{
	// Throw this exception from controller if you want to return user a meaningful error message
	public class UserFriendlyException: Exception
	{
		public UserFriendlyException(): base()
		{
		}

		public UserFriendlyException(string message): base(message)
		{
		}

		public UserFriendlyException(string message, Exception innerException): base(message, innerException)
		{
		}
	}
}
