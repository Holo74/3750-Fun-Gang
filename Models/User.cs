using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Assignment_1.Models
{
    public class User
    {
        public int Id { get; set; }

        //[RegularExpression(@"^[a-zA-Z\.]*+@+[a-zA-Z\.]*$")]
        [Required]
        public string Email { get; set; }

        // Requirements.  A single lower cases letter.  1 upper case letter.  1 decimal.  At least 1 special character.  Min length of 8 characters
        [Validators.PasswordValidation]
        [StringLength(80, MinimumLength = 6)]
        [Required]
        public string Password { get; set; }
        [Compare(otherProperty:"Password"), Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
