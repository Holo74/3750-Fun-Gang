using System.ComponentModel.DataAnnotations;

namespace Assignment_1.Models
{
    public class Registrations
    {
        [Required]
        public int ID { get; set; }
        public int? UserFK { get; set; }
        public int? ClassFK { get; set; }
        public int IsRegistered { get; set; }
    }
}