using Assignment_1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Assignment_1.Models
{
    public class Seeder
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            using (var context = new Assignment_1Context(serviceProvider.GetRequiredService<DbContextOptions<Assignment_1Context>>()))
            {
                if (context.User.Any())
                {
                    return;
                }

                context.User.Add(
                    new User {
                    Email = "me@email.com",
                    Password= "password",
                    FirstName= "James",
                    LastName= "Holden",
                    BirthDate= DateTime.Parse("01-01-2001"),
                    UserType = "Student"
                    });
                context.SaveChanges();
            }
        }
    }
}
