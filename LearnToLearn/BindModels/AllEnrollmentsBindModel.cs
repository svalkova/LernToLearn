using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.BindModels
{
    public class AllEnrollmentsBindModel
    {
        public string CourseName { get; set; }
        public double Grade { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}