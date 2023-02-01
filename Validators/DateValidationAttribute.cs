using System.ComponentModel.DataAnnotations;
public class DateValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        DateTime birthDate = Convert.ToDateTime(value);

        int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        int dob = int.Parse(birthDate.ToString("yyyyMMdd"));
        int age = (now - dob) / 10000;

        return age >= 16;
    }
}
