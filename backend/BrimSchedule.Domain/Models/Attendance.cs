using BrimSchedule.Domain.Models.Enum;

namespace BrimSchedule.Domain.Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public bool IsUserAttended { get; set; }
		public UserVote UserVote { get; set; }
		public string Notes { get; set; }

		public string UserId { get; set; }

		public int LessonId { get; set; }
		public virtual Lesson Lesson { get; set; }
	}
}
