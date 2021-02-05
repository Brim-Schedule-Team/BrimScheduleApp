using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BrimSchedule.Domain.EF;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Domain.Interfaces;

namespace BrimSchedule.Domain.Repositories
{
	public class AuditRepository: IRepository<Audit>
	{
		private readonly BrimScheduleContext _db;

		public AuditRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<Audit> GetAll()
		{
			return _db.Audit;
		}

		public Audit Get(int id)
		{
			return _db.Audit.Find(id);
		}

		public IEnumerable<Audit> Find(Func<Audit, bool> predicate)
		{
			return _db.Audit.Where(predicate).ToList();
		}

		public void Create(Audit item)
		{
			_db.Audit.Add(item);
		}

		public void Update(Audit item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var auditEntry = Get(id);
			if (auditEntry != null)
			{
				_db.Audit.Remove(auditEntry);
			}
		}
	}
}
