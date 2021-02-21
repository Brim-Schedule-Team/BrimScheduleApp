using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BrimSchedule.Domain.Models
{
	public class User: IdentityUser<int>
	{
		public string Login { get; set; }
		public bool IsVerified { get; set; }
		public bool IsDeleted { get; set; }

		public int RoleId { get; set; }
		public virtual Role Role { get; set; }
		public virtual Profile Profile { get; set; }
		public virtual ICollection<Attendance> Attendance { get; set; }
	}
}
