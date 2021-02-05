using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BrimSchedule.Domain.EF;
using BrimSchedule.Domain.Entities;
using BrimSchedule.Domain.Interfaces;

namespace BrimSchedule.Domain.Repositories
{
	public class UserSuggestionListRepository: IRepository<UserSuggestionList>
	{
		private readonly BrimScheduleContext _db;

		public UserSuggestionListRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<UserSuggestionList> GetAll()
		{
			return _db.UserSuggestionLists;
		}

		public UserSuggestionList Get(int id)
		{
			return _db.UserSuggestionLists.Find(id);
		}

		public IEnumerable<UserSuggestionList> Find(Func<UserSuggestionList, bool> predicate)
		{
			return _db.UserSuggestionLists.Where(predicate).ToList();
		}

		public void Create(UserSuggestionList item)
		{
			_db.UserSuggestionLists.Add(item);
		}

		public void Update(UserSuggestionList item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var userSuggestionList = Get(id);
			if (userSuggestionList != null)
			{
				_db.UserSuggestionLists.Remove(userSuggestionList);
			}
		}
	}
}
