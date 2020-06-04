using LearnToLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.Service
{
    public class TokenService : BaseService<Token>
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public TokenService(IValidationDictionary validationDictionary, UnitOfWork unitOfWork)
            :base(validationDictionary, unitOfWork)
        {
           
        }

        public Token GetByName(string name)
        {
            return unitOfWork.Get<Token>().GetAll(t => t.Name == name).SingleOrDefault();
        }
    }
}