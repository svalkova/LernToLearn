using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.BindModels
{
    public class CourseBindModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TeacherId { get; set; }
        public int Capacity { get; set; }
    }
}