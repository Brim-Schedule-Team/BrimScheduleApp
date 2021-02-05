using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BrimSchedule.Domain.EF;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Domain.Interfaces;

namespace BrimSchedule.Domain.Repositories
{
	public class UserRepository: IRepository<User>
	{
		private readonly BrimScheduleContext _db;

		public UserRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<User> GetAll()
		{
			return _db.Users;
		}

		public User Get(int id)
		{
			return _db.Users.Find(id);
		}

		public IEnumerable<User> Find(Func<User, bool> predicate)
		{
			return _db.Users.Where(predicate).ToList();
		}

		public void Create(User item)
		{
			_db.Users.Add(item);
		}

		public void Update(User item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var user = Get(id);
			if (user != null)
			{
				_db.Users.Remove(user);
			}
		}
	}
}
