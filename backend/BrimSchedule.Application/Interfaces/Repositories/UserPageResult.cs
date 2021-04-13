using System.Collections.Generic;
using BrimSchedule.Domain.Models;

namespace BrimSchedule.Application.Interfaces.Repositories
{
	public class UserPageResult
	{
		public ICollection<User> Users { get; set; } = new List<User>();
		public string NextPageToken { get; set; }
	}
}
