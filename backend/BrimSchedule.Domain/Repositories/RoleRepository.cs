using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BrimSchedule.Domain.EF;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Domain.Interfaces;

namespace BrimSchedule.Domain.Repositories
{
	public class RoleRepository: IRepository<Role>
	{
		private readonly BrimScheduleContext _db;

		public RoleRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<Role> GetAll()
		{
			return _db.Roles;
		}

		public Role Get(int id)
		{
			return _db.Roles.Find(id);
		}

		public IEnumerable<Role> Find(Func<Role, bool> predicate)
		{
			return _db.Roles.Where(predicate).ToList();
		}

		public void Create(Role item)
		{
			_db.Roles.Add(item);
		}

		public void Update(Role item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var role = Get(id);
			if (role != null)
			{
				_db.Roles.Remove(role);
			}
		}
	}
}
