using System;
using BrimSchedule.Domain.Models;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;

namespace BrimSchedule.Persistence.Repositories
{
	public class EFUnitOfWork: IUnitOfWork
	{
		private bool _disposed;
		private readonly BrimScheduleContext _db;
		private GenericRepository<User> _userRepository;
		private GenericRepository<Profile> _profileRepository;
		private GenericRepository<Role> _roleRepository;
		private GenericRepository<Lesson> _lessonRepository;
		private GenericRepository<Attendance> _attendanceRepository;
		private GenericRepository<Audit> _auditRepository;
		private GenericRepository<UserSuggestionList> _userSuggestionListRepository;

		public EFUnitOfWork(BrimScheduleContext dbContext)
		{
			_db = dbContext;
		}

		public IRepository<User> Users => _userRepository ??= new GenericRepository<User>(_db);
		public IRepository<Profile> Profiles => _profileRepository ??= new GenericRepository<Profile>(_db);
		public IRepository<Role> Roles => _roleRepository ??= new GenericRepository<Role>(_db);
		public IRepository<Lesson> Lessons => _lessonRepository ??= new GenericRepository<Lesson>(_db);
		public IRepository<Attendance> Attendance => _attendanceRepository ??= new GenericRepository<Attendance>(_db);
		public IRepository<Audit> Audit => _auditRepository ??= new GenericRepository<Audit>(_db);
		public IRepository<UserSuggestionList> UserSuggestionLists =>  _userSuggestionListRepository ??= new GenericRepository<UserSuggestionList>(_db);

		public void Save()
		{
			_db.SaveChanges();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_db.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
