using System;
using System.Collections.Generic;
using System.Linq;
using BrimSchedule.Domain.Models;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrimSchedule.Persistence.Repositories
{
	public class ClassRepository: IRepository<Class>
	{
		private readonly BrimScheduleContext _db;

		public ClassRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<Class> GetAll()
		{
			return _db.Classes;
		}

		public Class Get(int id)
		{
			return _db.Classes.Find(id);
		}

		public IEnumerable<Class> Find(Func<Class, bool> predicate)
		{
			return _db.Classes.Where(predicate).ToList();
		}

		public void Create(Class item)
		{
			_db.Classes.Add(item);
		}

		public void Update(Class item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var classEntry = Get(id);
			if (classEntry != null)
			{
				_db.Classes.Remove(classEntry);
			}
		}
	}
}
