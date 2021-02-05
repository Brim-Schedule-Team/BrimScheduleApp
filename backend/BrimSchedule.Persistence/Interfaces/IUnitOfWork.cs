using System;
using BrimSchedule.Domain.Entities;

namespace BrimSchedule.Persistence.Interfaces
{
	public interface IUnitOfWork: IDisposable
	{
		IRepository<User> Users { get; }
		IRepository<Profile> Profiles { get; }
		IRepository<Role> Roles { get; }
		IRepository<Class> Classes { get; }
		IRepository<Attendance> Attendance { get; }
		IRepository<Audit> Audit { get; }
		IRepository<UserSuggestionList> UserSuggestionLists { get; }
		void Save();
	}
}
