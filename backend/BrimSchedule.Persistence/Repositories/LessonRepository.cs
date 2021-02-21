using System;
using System.Collections.Generic;
using System.Linq;
using BrimSchedule.Domain.Models;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrimSchedule.Persistence.Repositories
{
	public class LessonRepository: IRepository<Lesson>
	{
		private readonly BrimScheduleContext _db;

		public LessonRepository(BrimScheduleContext context)
		{
			_db = context;
		}

		public IEnumerable<Lesson> GetAll()
		{
			return _db.Lessons;
		}

		public Lesson Get(int id)
		{
			return _db.Lessons.Find(id);
		}

		public IEnumerable<Lesson> Find(Func<Lesson, bool> predicate)
		{
			return _db.Lessons.Where(predicate).ToList();
		}

		public void Create(Lesson item)
		{
			_db.Lessons.Add(item);
		}

		public void Update(Lesson item)
		{
			_db.Entry(item).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			var lessonEntry = Get(id);
			if (lessonEntry != null)
			{
				_db.Lessons.Remove(lessonEntry);
			}
		}
	}
}
