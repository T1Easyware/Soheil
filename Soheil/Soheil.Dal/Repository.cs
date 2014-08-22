using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Soheil.Dal.Interfaces;

namespace Soheil.Dal
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly SoheilEdmContext _context;

        public Repository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

			_context = unitOfWork as SoheilEdmContext;
        }

        #region IRepository<TModel> Members

		public ParallelQuery<TModel> AsParallel()
        {
			return _context.CreateObjectSet<TModel>().AsParallel<TModel>();
		}

		public IQueryable<T> OfType<T>() { return _context.CreateObjectSet<TModel>().OfType<T>(); }
		public IQueryable<T> OfType<T>(params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
			q = includePath.Aggregate(q, (current, path) => current.Include(path));
			return q.OfType<T>();
		}

        public IEnumerable<TModel> GetAll()
        {
            return _context.CreateObjectSet<TModel>();//.ToList();
        }

		public IEnumerable<TModel> GetAll(params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
		    return includePath.Aggregate(q, (current, path) => current.Include(path));
		}

		public IEnumerable<TModel> Find(Expression<Func<TModel, bool>> where)
		{
			return _context.CreateObjectSet<TModel>().Where(where);
		}

		public IEnumerable<TModel> Find(Expression<Func<TModel, bool>> where, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
		    q = includePath.Aggregate(q, (current, path) => current.Include(path));
		    return q.Where(where);
		}

		public IEnumerable<TModel> Find(Expression<Func<TModel, bool>> where, Func<TModel, object> orderByKeySelector, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
			q = includePath.Aggregate(q, (current, path) => current.Include(path));
			return q.Where(where).OrderBy(orderByKeySelector);
		}

		public TModel Single(Expression<Func<TModel, bool>> where)
		{
			return _context.CreateObjectSet<TModel>().FirstOrDefault(where);
		}
		public TModel Single(Expression<Func<TModel, bool>> where, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
			q = includePath.Aggregate(q, (current, path) => current.Include(path));
			return q.FirstOrDefault(where);
		}

		public TModel First(Expression<Func<TModel, bool>> where)
		{
			return _context.CreateObjectSet<TModel>().First(where);
		}
		public TModel First(Expression<Func<TModel, bool>> where, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
			q = includePath.Aggregate(q, (current, path) => current.Include(path));
			return q.First(where);
		}

        public TModel FirstOrDefault(Expression<Func<TModel, bool>> where)
        {
            return _context.CreateObjectSet<TModel>().FirstOrDefault(where);
        }

		public TModel FirstOrDefault(Expression<Func<TModel, bool>> where, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
		    q = includePath.Aggregate(q, (current, path) => current.Include(path));
		    return q.FirstOrDefault(where);
		}
		public TModel FirstOrDefault(Expression<Func<TModel, bool>> where, Expression<Func<TModel, DateTime>> select)
		{
			return _context.CreateObjectSet<TModel>().OrderBy(select).FirstOrDefault(where);
		}
		public TModel FirstOrDefault(Expression<Func<TModel, bool>> where, Expression<Func<TModel, DateTime>> select, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
			q = includePath.Aggregate(q, (current, path) => current.Include(path));
			return q.OrderBy(select).FirstOrDefault(where);
		}

		public TModel LastOrDefault(Expression<Func<TModel, bool>> where, Expression<Func<TModel, DateTime>> select)
		{
			return _context.CreateObjectSet<TModel>().OrderByDescending(select).FirstOrDefault(where);
		}
		public TModel LastOrDefault(Expression<Func<TModel, bool>> where, Expression<Func<TModel, DateTime>> select, params string[] includePath)
		{
			System.Data.Objects.ObjectQuery<TModel> q = _context.CreateObjectSet<TModel>();
			q = includePath.Aggregate(q, (current, path) => current.Include(path));
			return q.OrderByDescending(select).FirstOrDefault(where);
		}

        public bool Exists(Expression<Func<TModel, bool>> where)
        {
            return _context.CreateObjectSet<TModel>().Any(where);
        }

        public void Delete(TModel entity)
        {
			var set = _context.CreateObjectSet<TModel>();
			set.DeleteObject(entity);
        }

		public void Add(TModel entity)
		{
			var set = _context.CreateObjectSet<TModel>();
			set.AddObject(entity);
		}

        #endregion

	}
}