using System;

namespace BrimSchedule.Domain.Models
{
	public class UserSuggestionList
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string UsersJson { get; set; }
		public DateTime CreatedOnUtc { get; set; }
	}
}
