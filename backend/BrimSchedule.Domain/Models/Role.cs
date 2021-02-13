﻿using System.Collections.Generic;

namespace BrimSchedule.Domain.Models
{
	public class Role
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<User> Users { get; set; }
	}
}
