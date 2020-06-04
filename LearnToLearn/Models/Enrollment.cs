using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnToLearn.Models
{
    public class Enrollment : BaseModel
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public double Grade { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual Course Course { get; set; }
    }
}