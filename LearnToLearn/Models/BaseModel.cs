using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LearnToLearn.Models
{
    public class BaseModel
    {
        //[ScaffoldColumn(false)]
        public int Id { get; set; }
    }
}