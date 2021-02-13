using BrimSchedule.Domain.Models.Enum;

namespace BrimSchedule.Domain.Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public bool IsUserAttended { get; set; }
		public UserVote UserVote { get; set; }
		public string Notes { get; set; }

		public int UserId { get; set; }
		public virtual User User { get; set; }

		public int ClassId { get; set; }
		public virtual Class Class { get; set; }
	}
}
