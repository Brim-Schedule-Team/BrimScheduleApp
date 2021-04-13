using System;
using BrimSchedule.Domain.Models;

namespace BrimSchedule.Application.Interfaces.Repositories
{
	public interface IUnitOfWork: IDisposable
	{
		IRepository<Profile> Profiles { get; }
		IRepository<Lesson> Lessons { get; }
		IRepository<Attendance> Attendance { get; }
		IRepository<Audit> Audit { get; }
		IRepository<UserSuggestionList> UserSuggestionLists { get; }
		void Save();
	}
}
