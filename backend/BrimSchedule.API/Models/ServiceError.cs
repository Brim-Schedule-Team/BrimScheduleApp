using System;

namespace BrimSchedule.API.Models
{
	public class ServiceError
	{
		public Guid ErrorId { get; set; }
		public string ErrorMessage { get; set; }
	}
}
