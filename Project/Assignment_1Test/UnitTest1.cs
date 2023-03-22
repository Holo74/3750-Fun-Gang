using Microsoft.EntityFrameworkCore;
using Assignment_1;
using Assignment_1.Models;
using Assignment_1.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment_1Test
{
    [TestClass]
    public class UnitTest1
    {
        private readonly Assignment_1Context _context;

        public UnitTest1()
        {

            DbContextOptions<Assignment_1Context> options = new DbContextOptions<Assignment_1Context>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Server=titan.cs.weber.edu,10433;Database=LMS_FunGang;User Id=LMS_FunGang;Password=FunGang!5;", null);
            _context = new Assignment_1Context((DbContextOptions<Assignment_1Context>)builder.Options);
        }

        [TestMethod]
        public async Task CanInstructorCreateClassTest()
        {
            //find instructor id that exists (id = 41 is testguy@gmail.com)
            //query for how many courses the professor is teaching

            var user = _context.User.Where(x => x.Id == 41).ToList();
            var numOfClassesBefore = _context.Class.Where(c => c.UserId == user[0].Id).ToList();
            int lengthBefore = numOfClassesBefore.Count();

            //make the instructor create a course (pass all information for course creation)
            //query for how many courses the professor is teaching (should be one more than last time)

            Class course = new Class(user[0].Id, "ENGL", 2010, "Writing", 4, "Elizabeth Hall", "M | W", DateTime.Now, DateTime.Now.AddHours(1));
            _context.Class.Add(course);
            await _context.SaveChangesAsync();

            //if thats true, pass. else, fail

            var numOfClassesAfter = _context.Class.Where(c => c.UserId == user[0].Id).ToList();
            int lengthAfter = numOfClassesAfter.Count();

            Assert.AreEqual(lengthAfter, lengthBefore + 1);

            _context.Class.Remove(course);
            await _context.SaveChangesAsync();
        }

        [TestMethod]
		public async Task StudentCanRegisterForCourse()
		{
			//find student id that exists (id = 43 is teststud)
			//query for how many courses the student is signed up for

			var user = _context.User.Where(x => x.Id == 43).ToList();
			var numOfClassesBefore = _context.Registrations.Where(c => c.UserFK == user[0].Id && c.IsRegistered == 1).ToList();
			int lengthBefore = numOfClassesBefore.Count();

			//find a class already in the registration table
			var registrationList = _context.Registrations.Where(d => d.ID == 27).ToList();

            if (registrationList.Count> 0)
            {
                var registration = registrationList[0];
                registration.IsRegistered = 1; 
                //make the student register for a course
			    //query for how many courses the student is registered for (should be one more than last time)
			    await _context.SaveChangesAsync();

			    //if thats true, pass. else, fail

			    var numOfClassesAfter = _context.Registrations.Where(c => c.UserFK == user[0].Id && c.IsRegistered == 1).ToList();
			    int lengthAfter = numOfClassesAfter.Count();

			    Assert.AreEqual(lengthAfter, lengthBefore + 1);

				registration.IsRegistered = 0;
				//_context.Registrations.Remove()
				await _context.SaveChangesAsync();
            }



			
		}
	}
}