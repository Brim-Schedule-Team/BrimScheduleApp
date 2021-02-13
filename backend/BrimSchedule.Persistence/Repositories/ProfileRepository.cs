using System;
using System.Collections.Generic;
using System.Linq;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrimSchedule.Persistence.Repositories
{
	public class ProfileRepository: IRepository<Profile>
	{
		private readonly BrimScheduleContext _db;

		public ProfileRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<Profile> GetAll()
		{
			return _db.Profiles;
		}

		public Profile Get(int id)
		{
			return _db.Profiles.Find(id);
		}

		public IEnumerable<Profile> Find(Func<Profile, bool> predicate)
		{
			return _db.Profiles.Where(predicate).ToList();
		}

		public void Create(Profile item)
		{
			_db.Profiles.Add(item);
		}

		public void Update(Profile item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var profile = Get(id);
			if (profile != null)
			{
				_db.Profiles.Remove(profile);
			}
		}
	}
}
