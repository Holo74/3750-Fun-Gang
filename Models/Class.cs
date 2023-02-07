using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Assignment_1.Models
{
    public class Class
    {
        // NOT COMPLETELY ACCURATE YET **************************************************************
        [Required]
        public int ClassId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public int CourseNumber { get; set; }
        [Required]
        public string CourseName { get; set; }
        [Required]
        public int NumOfCredits { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string DaysOfWeek { get; set; }
        [Required]
        public string TimeOfDay { get; set; }
    }
}
