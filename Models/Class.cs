using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Assignment_1.Models
{
    public class Class
    {
        // NOT COMPLETELY ACCURATE BY ANY MEANS YET **************************************************************
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string Department { get; set; }
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public int NumOfCredits { get; set; }
        public string Location { get; set; }
        public string DaysOfWeek { get; set; }
        public string TimeOfDay { get; set; }
    }
}
