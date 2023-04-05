using Microsoft.EntityFrameworkCore;
using Assignment_1;
using Assignment_1.Models;
using Assignment_1.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment_1.Controllers;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Stripe;
using Assignment_1.Controllers;
using System.Security.Cryptography;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

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

            Class course = new Class();
            course.Department = "CS";
            course.CourseNumber = 3456;
            course.CourseName = "Dummy Class";
            course.NumOfCredits = 3;
            course.Location = "The Place";
            course.StartTime = DateTime.Now;
            course.EndTime = DateTime.Now.AddHours(1);

            List<string> days = new List<string>();
            days.Add("M");
            days.Add("W");
            ICollection<string> day = days;

            ClassesController classes = new ClassesController(_context);
            await classes.CreateMain(user[0].Id, course, days);

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

            if (registrationList.Count > 0)
            {
                //var registration = registrationList[0];
                StudentRegistrationController registrations = new StudentRegistrationController(_context);
                //27 is a class made for unit testing
                await registrations.RegisterForClassLogic(43, 16);
                //registration.IsRegistered = 1; 
                //make the student register for a course
                //query for how many courses the student is registered for (should be one more than last time)
                //await _context.SaveChangesAsync();

                //if thats true, pass. else, fail

                var numOfClassesAfter = _context.Registrations.Where(c => c.UserFK == user[0].Id && c.IsRegistered == 1).ToList();
                int lengthAfter = numOfClassesAfter.Count();

                Assert.AreEqual(lengthAfter, lengthBefore + 1);

                await registrations.RegisterForClassLogic(43, 16);
                //_context.Registrations.Remove()
                await _context.SaveChangesAsync();
            }
        }
        [TestMethod]
        public async Task AssignmentCanBeGraded()
        {
            var submissions = (from a in _context.AssignmentSubmissions select a).ToList();
            int Tries = 3;//maximum submissions to test

            //test first (Tries) submissions retrieved
            foreach(var submission in submissions )
            {
                if(Tries > 0)
                {
                    ClassAssignmentsController c = new ClassAssignmentsController(_context);

                    //random point value to assign
                    int randomPoints = new Random().Next(1, 11);
					
                    //call set points function on current submission with teh random value
					await c.SetPoints(randomPoints, submission.Id);

                    AssignmentSubmissions a = _context.AssignmentSubmissions.Where(x => x.Id == submission.Id).FirstOrDefault();

                    Assert.IsNotNull(a);

                    if(a != null)
                    {
                        Assert.AreEqual(randomPoints, a.Points);
                    }

                    Tries--;
                }
                else
                {
                    break;
                }

            }
           
        }


        [TestMethod]
        public async Task NotificationsWork()
        {
            int id = 14;
            User u = _context.User.Where(x => x.Id == id).FirstOrDefault();

            u.LastedLoggedIn = new DateTime(2022, 1, 1);

            _context.User.Update(u);
            await _context.SaveChangesAsync();

            int count = _context.AssignmentSubmissions.Where(x => x.UserFK == id).Count();

            count +=
                (from reg in _context.Registrations
                 join newAssignments in _context.ClassAssignments
                 on reg.ClassFK equals newAssignments.ClassId
                 where reg.UserFK == id
                 where newAssignments.CreatedDate > u.LastedLoggedIn
                 select newAssignments.ClassId).Count();

            Assert.AreEqual(count, new UsersController(_context, null, null).GetNotifications(u).Result.Count);



        }

        [TestMethod]
        public void SeleniumCreateAccount()
        {

            IWebDriver driver = new FirefoxDriver();

            driver.Navigate().GoToUrl("https://notebook-cs3750.azurewebsites.net/Login/");

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            var textBox = driver.FindElement(By.ClassName("btn-secondary"));
            textBox.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            var signupBut = driver.FindElement(By.ClassName("btn-primary"));
            signupBut.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            string emailError = driver.FindElement(By.Id("Email-error")).Text;

            Assert.IsTrue(emailError.Length > 0);

            driver.Quit();
        }
    }
}