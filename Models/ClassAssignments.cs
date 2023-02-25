using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Assignment_1.Models
{
    public class ClassAssignments
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int ClassId { get; set; }
        public string? AssignmentTitle { get; set; }
        public string? Description { get; set; }
        public int? MaxPoints { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        [DataType(DataType.Time)]
        public DateTime? DueTime { get; set; }
        public string SubmissionType { get; set; }


    }
}
