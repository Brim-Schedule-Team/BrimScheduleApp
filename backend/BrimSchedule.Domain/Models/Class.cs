using System;
using System.Collections.Generic;
using BrimSchedule.Domain.Entities.Enum;

namespace BrimSchedule.Domain.Entities
{
	public class Class
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime StartsOnUtc { get; set; }
		public ClassStatus Status { get; set; }
		public int? DurationMin { get; set; }
		public string Description { get; set; }
		public bool IsDeleted { get; set; }

		public virtual ICollection<Attendance> Attendance { get; set; }
	}
}
