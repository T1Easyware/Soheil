using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Soheil.Dal.Interfaces
{
    /// <summary>
    /// Defines a method that provides data storing and loading.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IRepository<TModel> where TModel : class
    {
		ParallelQuery<TModel> AsParallel();

        IEnumerable<TModel> GetAll();
        IEnumerable<TModel> Find(Expression<Func<TModel, bool>> where);
        TModel Single(Expression<Func<TModel, bool>> where);
        TModel First(Expression<Func<TModel, bool>> where);
        TModel FirstOrDefault(Expression<Func<TModel, bool>> where);
        bool Exists(Expression<Func<TModel, bool>> where);

        void Delete(TModel entity);
        void Add(TModel entity);
    }
}