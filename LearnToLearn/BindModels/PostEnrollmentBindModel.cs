using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.BindModels
{
    public class PostEnrollmentBindModel
    {
        public int CourseId { get; set; }
        public double Grade { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}