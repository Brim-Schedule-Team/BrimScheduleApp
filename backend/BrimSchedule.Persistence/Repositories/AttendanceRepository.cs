using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;

namespace BrimSchedule.Persistence.Repositories
{
	public class AttendanceRepository: IRepository<Attendance>
	{
		private readonly BrimScheduleContext _db;

		public AttendanceRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<Attendance> GetAll()
		{
			return _db.Attendance;
		}

		public Attendance Get(int id)
		{
			return _db.Attendance.Find(id);
		}

		public IEnumerable<Attendance> Find(Func<Attendance, bool> predicate)
		{
			return _db.Attendance.Where(predicate).ToList();
		}

		public void Create(Attendance item)
		{
			_db.Attendance.Add(item);
		}

		public void Update(Attendance item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var attendanceEntry = Get(id);
			if (attendanceEntry != null)
			{
				_db.Attendance.Remove(attendanceEntry);
			}
		}
	}
}
