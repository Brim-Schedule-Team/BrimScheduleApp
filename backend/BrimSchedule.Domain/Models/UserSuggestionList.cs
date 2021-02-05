using System;

namespace BrimSchedule.Domain.Entities
{
	public class UserSuggestionList
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string UsersJson { get; set; }
		public DateTime CreatedOnUtc { get; set; }
	}
}
