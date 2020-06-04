using LearnToLearn.DataAccess;
using LearnToLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn
{
    public class UnitOfWork : IDisposable 
    {
        private LearnToLearnContext context = new LearnToLearnContext();
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public Repository<T> Get<T>() where T : BaseModel
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as Repository<T>;
            }
            else
            {
                Repository<T> repo = new Repository<T>(context);
                repositories.Add(typeof(T), repo);
                return repo;
            }
            
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}