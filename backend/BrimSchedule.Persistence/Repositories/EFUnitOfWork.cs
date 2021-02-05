using System;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;

namespace BrimSchedule.Persistence.Repositories
{
	public class EFUnitOfWork: IUnitOfWork
	{
		private bool _disposed;
		private readonly BrimScheduleContext _db;
		private UserRepository _userRepository;
		private ProfileRepository _profileRepository;
		private RoleRepository _roleRepository;
		private ClassRepository _classRepository;
		private AttendanceRepository _attendanceRepository;
		private AuditRepository _auditRepository;
		private UserSuggestionListRepository _userSuggestionListRepository;

		public EFUnitOfWork(string connectionString)
		{
			_db = new BrimScheduleContext(connectionString);
		}

		public IRepository<User> Users => _userRepository ??= new UserRepository(_db);
		public IRepository<Profile> Profiles => _profileRepository ??= new ProfileRepository(_db);
		public IRepository<Role> Roles => _roleRepository ??= new RoleRepository(_db);
		public IRepository<Class> Classes => _classRepository ??= new ClassRepository(_db);
		public IRepository<Attendance> Attendance => _attendanceRepository ??= new AttendanceRepository(_db);
		public IRepository<Audit> Audit => _auditRepository ??= new AuditRepository(_db);
		public IRepository<UserSuggestionList> UserSuggestionLists =>  _userSuggestionListRepository ??= new UserSuggestionListRepository(_db);

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
