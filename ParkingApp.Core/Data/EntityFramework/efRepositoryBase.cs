using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;


using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Core.Data.EntityFramework
{
    public class efRepositoryBase<TEntity> : IEntityRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public efRepositoryBase(DbContext dbContext)
        {


            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            var activeEntity = _dbContext.Entry(entity);
            activeEntity.State = EntityState.Added;

        }

        public void Delete(TEntity entity)
        {

            DbEntityEntry dbEntityEntry = _dbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return;
            else
            {
                if (entity.GetType().GetProperty("IsDelete") != null)
                {
                    TEntity _entity = entity;
                    _entity.GetType().GetProperty("IsDelete").SetValue(_entity, true);

                    this.Update(_entity);
                }
                else
                {
                    Delete(entity);
                }
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).SingleOrDefault();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }
        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public bool SaveChanges()
        {
            var result = false;
            try
            {

                _dbContext.SaveChanges();
                result = true;

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public void Update(TEntity entity)
        {
            var activeEntity = _dbContext.Entry(entity);
            activeEntity.State = EntityState.Modified;
        }
    }
}
