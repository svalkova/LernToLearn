using LearnToLearn.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LearnToLearn.DataAccess
{
    public class Repository<T>  where T : BaseModel
    {
        protected LearnToLearnContext context;
        public DbSet<T> dbSet;

        public Repository(LearnToLearnContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }
        public Repository() : this(new LearnToLearnContext())
        {
        }

        public List<T> GetAll()
        {
            return dbSet.ToList();
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return dbSet.Where(filter).ToList();
        }

        public T GetById(int? id)
        {
            return dbSet.Find(id);
        }

        public void Insert(T t)
        {
            context.Entry(t).State = EntityState.Added;
        }

        public void Delete(T t)
        {
            dbSet.Remove(t);
        }

        public void Update(T t)
        {
            context.Entry(t).State = EntityState.Modified;
        }
    }
}