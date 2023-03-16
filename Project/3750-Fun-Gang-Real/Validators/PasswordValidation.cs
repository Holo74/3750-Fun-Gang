using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Assignment_1.Validators
{
    public class PasswordValidation : ValidationAttribute
    {
        /// <summary>
        /// Input as regex for the password requirements.
        /// </summary>
        public RegexAndErrorPair[] PasswordRequirements { get; set; } =
        {
            /*
            new RegexAndErrorPair()
            {
                Error = "More than 8 characters",
                Regex = "^.{6,}$"
            },
            */
            new RegexAndErrorPair()
            {
                Error = "At least one upper case character",
                Regex = "(?=[^A-Z]*[A-Z]).{1,}"
            },
            new RegexAndErrorPair()
            {
                Error = "At least one lower case character",
                Regex = "(?=[^a-z]*[a-z]).{1,}"
            },
            new RegexAndErrorPair()
            {
                Error = "At least one special character",
                Regex = "(?=\\w*\\d*[@#$%^&*]).{1,}"
            },
            new RegexAndErrorPair()
            {
                Error = "At least one Decimal",
                Regex = "(?=\\D*\\d).{1,}"
            }
        };

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }

        protected override ValidationResult IsValid(object obj, ValidationContext validationContext)
        {
            if (obj == null)
                return new ValidationResult("Not found");
            if (obj is string pass)
            {
                // This'll loop through the requirements and then output a proper error.
                string errorMessage = "";
                bool passing = true;
                foreach (RegexAndErrorPair pair in PasswordRequirements)
                {
                    if(!Regex.IsMatch(pass, pair.Regex))
                    {
                        errorMessage += "" + pair.Error + ".  ";
                        passing = false;
                    }
                }
                if(passing)
                {
                    return ValidationResult.Success;
                }
                errorMessage += "";
                return new ValidationResult(errorMessage);
            }
            return new ValidationResult("Incorrect data passed");
        }

        public struct RegexAndErrorPair
        {
            public RegexAndErrorPair()
            {
                Regex = "";
                Error = "";
            }
            public string Regex { get; set; }
            public string Error { get; set; }
        }
    }
}
