using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        List<T> GatAll(string? include = null);

        // CRUD
        public IEnumerable<T> Get(Expression<Func<T, object>>[]? includeProp = null, Expression<Func<T, bool>>? expression = null);

        T? GetOne(Expression<Func<T, bool>> expression);

        T? GetById(int entityId);
        T? GetFirstOrDefault(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[]? includeProp = null);

        void create(T entity);



        void Edit(T entity);



        void Delete(T entity);


        void commit();
        int Count(Expression<Func<T, bool>>? expression = null);

    }
}
