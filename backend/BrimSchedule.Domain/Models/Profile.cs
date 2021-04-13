using System;

namespace BrimSchedule.Domain.Models
{
	public class Profile
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Patronymic { get; set; }
		public DateTime BirthDate { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string OtherContact { get; set; }
		public string Notes { get; set; }

		public string UserId { get; set; }
	}
}
