using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace LearnToLearn.Service
{
    public class ModelStateWrapper : IValidationDictionary
    {
        private ModelStateDictionary modelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            this.modelState = modelState;
        }

        public void AddError(string key, string errorMessage)
        {
            modelState.AddModelError(key, errorMessage);
        }

        public bool IsValid
        {
            get { return modelState.IsValid; }
        }

    }
}