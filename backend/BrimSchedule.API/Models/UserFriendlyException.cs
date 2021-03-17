using System;

namespace BrimSchedule.Domain.Request
{
	public class UserFriendlyException: Exception
	{
		public UserFriendlyException(string message, Exception innerException): base(message, innerException)
		{
		}

		public UserFriendlyException(string message): base(message)
		{
		}
	}
}
