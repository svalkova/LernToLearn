﻿namespace LearnToLearn.Service
{
    public interface IValidationDictionary
    {
        void AddError(string key, string errorMessage);
        bool IsValid { get; }
    }
}