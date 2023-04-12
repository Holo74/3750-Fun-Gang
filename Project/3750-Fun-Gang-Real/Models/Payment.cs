using System.ComponentModel.DataAnnotations;

namespace Assignment_1.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserFK { get; set; }
        public decimal Amount { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }
}
