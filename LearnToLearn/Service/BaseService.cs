using LearnToLearn;
using LearnToLearn.DataAccess;
using LearnToLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LearnToLearn.Service
{
    public class BaseService <T> where T : BaseModel
    {
        public IValidationDictionary validationDictionary;
        private UnitOfWork unitOfWork = new UnitOfWork();

        public BaseService(IValidationDictionary validationDictionary, UnitOfWork unitOfWork)
        {
            this.validationDictionary = validationDictionary;
            this.unitOfWork = unitOfWork;
        }

        public virtual bool ValidateModel(T t)
        {
            return validationDictionary.IsValid;
        }

        public T GetById(int? id)
        {
            return unitOfWork.Get<T>().GetById(id);
        }

        public List<T> GetAll()
        {
            return unitOfWork.Get<T>().GetAll();
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return unitOfWork.Get<T>().GetAll(filter);
        }

        public bool Create(T t)
        {
            if (!ValidateModel(t))
                return false;
            try
            {
                unitOfWork.Get<T>().Insert(t);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Update(T t)
        {
            if (!ValidateModel(t))
                return false;
            try
            {
                unitOfWork.Get<T>().Update(t);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Delete(int? id)
        {
            T t = unitOfWork.Get<T>().GetById(id.Value);
            try
            {
                unitOfWork.Get<T>().Delete(t);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Save()
        {
            unitOfWork.Save();
        }

    }
}
