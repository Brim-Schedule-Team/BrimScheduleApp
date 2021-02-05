using System.Collections.Generic;

namespace BrimSchedule.Domain.Entities
{
	public class Role
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<User> Users { get; set; }
	}
}
