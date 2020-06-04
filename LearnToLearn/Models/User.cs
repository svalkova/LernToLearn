using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.Models
{
    public class User : BaseModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsTeacher { get; set; }

        public virtual List<Enrollment> Enrollments { get; set; }
        public virtual List<Course> Courses { get; set; }
        public virtual List<Token> Tokens { get; set; }

    }
}