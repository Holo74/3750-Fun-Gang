using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Assignment_1.Models
{
    public class User
    {
        public int Id { get; set; }

        //[RegularExpression(@"^[a-zA-Z\.]*+@+[a-zA-Z\.]*$")]
        [Required]
        public string Email { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [DateValidation(ErrorMessage = "User Age must be at least 16")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate{ get; set;}
    }
}
