using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BrimSchedule.Application.Interfaces.Repositories;
using BrimSchedule.Persistence.EF;
using Microsoft.EntityFrameworkCore;

namespace BrimSchedule.Persistence.Repositories
{
	public class Repository<TEntity>: IRepository<TEntity>
		where TEntity : class
	{
		private readonly BrimScheduleContext _context;
		private readonly DbSet<TEntity> _dbSet;

		public Repository(BrimScheduleContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_dbSet = context.Set<TEntity>();
		}

		public virtual IEnumerable<TEntity> Get(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			params string[] includeProperties)
		{
			IQueryable<TEntity> query = _dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}

			return orderBy?.Invoke(query).ToList() ?? query.ToList();
		}

		public virtual TEntity GetById(object id)
		{
			return _dbSet.Find(id);
		}

		public virtual void Insert(TEntity entity)
		{
			_dbSet.Add(entity);
		}

		public virtual void Delete(object id)
		{
			TEntity entityToDelete = _dbSet.Find(id);
			Delete(entityToDelete);
		}

		public virtual void Delete(TEntity entityToDelete)
		{
			if (_context.Entry(entityToDelete).State == EntityState.Detached)
			{
				_dbSet.Attach(entityToDelete);
			}
			_dbSet.Remove(entityToDelete);
		}

		public virtual void Update(TEntity entityToUpdate)
		{
			_dbSet.Attach(entityToUpdate);
			_context.Entry(entityToUpdate).State = EntityState.Modified;
		}
	}
}
