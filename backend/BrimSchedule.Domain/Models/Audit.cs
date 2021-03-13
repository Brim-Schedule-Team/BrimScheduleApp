using System;

namespace BrimSchedule.Domain.Models
{
	public class Audit
	{
		public int Id { get; set; }
		public string Action { get; set; }
		public DateTime DateTimeUtc { get; set; }
		public string Login { get; set; }
		public string IpAddress { get; set; }
		public string Details { get; set; }
	}
}
