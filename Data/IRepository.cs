using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public interface IRepository<T> where T : class
    {
        void Add(T item);
        void Update(T item);
        void Delete(T item);
        Task<bool> SaveAll();
        IQueryable<T> GetAll();
        //T Single<T>(Expression<Func<T, bool>> expression) where T : class;
        //Task<T> GetByField(Expression<Func<T, bool>> predicate);

    }
}
