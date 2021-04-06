using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatDoMhund.Data.Repository.Abstract
{
    public abstract class RepositoryBase<TEntity> where TEntity : class
    {
        protected readonly MhundDbContext _db;

        protected RepositoryBase(MhundDbContext db)
        {
            this._db = db;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return this._db.Set<TEntity>().ToList();
        }

        public virtual SaeResponseRepository<TEntity> GetById(int id)
        {
            TEntity model = this._db.Set<TEntity>().Find(id);
            return new SaeResponseRepository<TEntity>
            {
                Status = model != null,
                Content = model
            };
        }

        public SaeResponseRepository<TEntity> Add(TEntity obj)
        {
            SaeResponseRepository<TEntity> response = new SaeResponseRepository<TEntity>();
            try
            {
                response.Content = this._db.Set<TEntity>().Add(obj).Entity;

                response.Status = this.SaveAll();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        public async Task<SaeResponseRepository<TEntity>> AddAsync(TEntity obj)
        {
            SaeResponseRepository<TEntity> response = new SaeResponseRepository<TEntity>();
            try
            {
                await this._db.Set<TEntity>().AddAsync(obj);

                response.Status = this.SaveAll();
                response.Content = obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        public SaeResponseRepository<TEntity> Update(TEntity obj)
        {
            SaeResponseRepository<TEntity> response = new SaeResponseRepository<TEntity>();
            try
            {
                response.Content = this._db.Set<TEntity>().Update(obj).Entity;

                response.Status = this.SaveAll();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return response;
        }

        public SaeResponseRepository<TEntity> Delete(TEntity obj)
        {
            SaeResponseRepository<TEntity> response = new SaeResponseRepository<TEntity>();
            try
            {
                response.Content = this._db.Set<TEntity>().Remove(obj).Entity;
                response.Status = this.SaveAll();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return response;
        }

        public bool SaveAll() => this._db.SaveChanges() > 0;

        public void Dispose() => this._db.Dispose();
    }
}
