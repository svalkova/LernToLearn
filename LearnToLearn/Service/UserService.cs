using LearnToLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.Service
{
    public class UserService : BaseService<User>
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public UserService(IValidationDictionary validationDictionary, UnitOfWork unitOfWork)
            :base(validationDictionary, unitOfWork)
        {

        }

        public User GetByEmailAndPassword(User user)
        {
            return unitOfWork.Get<User>().GetAll(u => u.Email == user.Email && u.Password == user.Password).SingleOrDefault();
        }
    }
}