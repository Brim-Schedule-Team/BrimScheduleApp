using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BrimSchedule.Domain.Models
{
	public class User
	{
		public string Id { get; set; }
		public string PhoneNumber { get; set; }
		public bool Disabled { get; set; }
		public ICollection<string> Roles { get; set; } = new List<string>();

		public Profile Profile { get; set; }
	}
}
