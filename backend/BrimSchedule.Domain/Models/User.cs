using System.Collections.Generic;

namespace BrimSchedule.Domain.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Login { get; set; }
		public bool IsVerified { get; set; }
		public bool IsDeleted { get; set; }

		public int RoleId { get; set; }
		public virtual Role Role { get; set; }
		public virtual Profile Profile { get; set; }
		public virtual ICollection<Attendance> Attendance { get; set; }
	}
}
