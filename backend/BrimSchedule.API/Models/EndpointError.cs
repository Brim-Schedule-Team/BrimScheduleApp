using System;
using System.Text.Json.Serialization;

namespace BrimSchedule.API.Models
{
	public class EndpointError
	{
		public Guid ErrorId { get; set; }
		public string ErrorMessage { get; set; }

		[JsonIgnore]
		public Exception OriginalException { get; set; }
	}
}
