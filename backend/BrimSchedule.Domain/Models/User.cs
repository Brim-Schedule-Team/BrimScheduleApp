using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BrimSchedule.Domain.Models
{
	public class User
	{
		public string Id { get; set; }
		public string PhoneNumber { get; set; }
		public bool Disabled { get; set; }
		public string Role { get; set; }

		public Profile Profile { get; set; }
	}
}
