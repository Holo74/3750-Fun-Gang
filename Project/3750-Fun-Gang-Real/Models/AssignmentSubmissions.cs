using System.ComponentModel.DataAnnotations;

namespace Assignment_1.Models
{
    public class AssignmentSubmissions
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserFK { get; set; }
        [Required]
        public int AssignmentFK { get; set; }
        [Required]
        public int ClassFK { get; set; }
        public int? Points { get; set; }
        public string? Data { get; set; }
    
        //[DataType(DataType.Date)]
        public DateTime? SubmitDate { get; set; }
        [DataType(DataType.Time)]
        public DateTime? SubmitTime { get; set; }
    }
}
