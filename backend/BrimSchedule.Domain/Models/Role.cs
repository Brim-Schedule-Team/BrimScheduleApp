using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BrimSchedule.Domain.Models
{
	public class Role: IdentityRole<int>
	{
		public virtual ICollection<User> Users { get; set; }
	}
}
