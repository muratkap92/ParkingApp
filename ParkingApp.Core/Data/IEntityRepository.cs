using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Core.Data
{

    public interface IEntityRepository<T> where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        T GetById(int id);
        T Get(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);

        bool SaveChanges();
    }

}
