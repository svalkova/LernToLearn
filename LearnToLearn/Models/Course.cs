using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LearnToLearn.Models
{
    public class Course : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [ForeignKey("User")]
        public int? TeacherId { get; set; }
        [Required]
        public bool isVisible { get; set; }
        [Range(1,100, ErrorMessage ="Capacity must be between 1 and 100!")]
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual List<Enrollment> Enrollments { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}