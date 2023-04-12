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
using Microsoft.Extensions.Caching.Memory;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Stripe.BillingPortal;

namespace Assignment_1Test
{
    [TestClass]
    public class UnitTest1
    {
        private readonly Assignment_1Context _context;
        private IMemoryCache _cache;

        public UnitTest1()
        {

            DbContextOptions<Assignment_1Context> options = new DbContextOptions<Assignment_1Context>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=tcp:fun-gang.database.windows.net,1433;Initial Catalog=db;Persist Security Info=False;User ID=databaseAdmin;Password=DoNotForgetTh1s!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", null);
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

            ClassesController classes = new ClassesController(_context, _cache);
            await classes.CreateMain(user[0].Id, course, days);

            //if thats true, pass. else, fail

            var numOfClassesAfter = _context.Class.Where(c => c.UserId == user[0].Id).ToList();
            int lengthAfter = numOfClassesAfter.Count();

            Assert.AreEqual(lengthAfter, lengthBefore + 1);

            _context.Class.Remove(course);
            await _context.SaveChangesAsync();
        }
		[TestMethod]
        public async Task TeacherCanCreateAssignment()
        {
            var course =  _context.Class.Where(x => x.ClassId ==13).ToList();
            var numOfAssignmentBefore = _context.ClassAssignments.Where(c => c.ClassId == course[0].ClassId).ToList();
            int lengthBefore = numOfAssignmentBefore.Count();

            ClassAssignments assignments = new ClassAssignments();
            assignments.AssignmentTitle = "TesterTest";
            assignments.Description = "DescriptionTest";
            assignments.MaxPoints = 100;
            assignments.DueDate = DateTime.Now;
            assignments.DueTime = DateTime.Now;
            assignments.SubmissionType = "doc";

            ClassAssignmentsController cac = new ClassAssignmentsController(_context, _cache);
            await cac.CreateMain(course[0].ClassId, assignments);

            var numOfAssignmentsAfter = _context.ClassAssignments.Where(c =>c.ClassId == course[0].ClassId).ToList();
            int lengthAfter = numOfAssignmentsAfter.Count();

            Assert.AreEqual(lengthAfter, lengthBefore + 1);

            _context.ClassAssignments.Remove(assignments);
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
                StudentRegistrationController registrations = new StudentRegistrationController(_context, _cache);
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
                    ClassAssignmentsController c = new ClassAssignmentsController(_context, _cache);

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
		[TestMethod]
		public void StudentCanRegisterForCourseSelenium()
		{

			IWebDriver driver = new ChromeDriver();
		    //https://localhost:7099/Login/Index/
			driver.Navigate().GoToUrl("https://localhost:7099/Login/Index/");

			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

			var textBox = driver.FindElement(By.Name("Email"));
			textBox.Click();
            textBox.SendKeys("teststud");

			var passBox = driver.FindElement(By.Name("Password"));
			passBox.Click();
            passBox.SendKeys("Waffle1#");

            var loginbtn = driver.FindElement(By.ClassName("btn-primary"));
            loginbtn.Click();

			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            var regbtn = driver.FindElement(By.LinkText("Registration"));
            regbtn.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(2000);

            var coursebtn = driver.FindElement(By.Name("Register"));
            coursebtn.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(2000);

            var homebtn = driver.FindElement(By.LinkText("Home"));
            homebtn.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            string isclassthere = driver.FindElement(By.ClassName("card")).Text;

            Assert.IsTrue(isclassthere.Length > 0);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

			regbtn = driver.FindElement(By.LinkText("Registration"));
			regbtn.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            coursebtn = driver.FindElement(By.Name("Register"));
			coursebtn.Click();

            driver.Quit();
		}
        [TestMethod]
        public void CanInstructorCreateClassTestSelenium()
        {
            // Go to Website
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://notebook-cs3750.azurewebsites.net/Login/");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            // Login
            var textBox = driver.FindElement(By.Name("Email"));
            textBox.Click();
            textBox.SendKeys("testtest");

            var passBox = driver.FindElement(By.Name("Password"));
            passBox.Click();
            passBox.SendKeys("thepassword");

            var loginbtn = driver.FindElement(By.ClassName("btn-primary"));
            loginbtn.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1000);

            // Go To Classes and Create Page
            var classes = driver.FindElement(By.Name("classes"));
            classes.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            var createClass = driver.FindElement(By.Name("create"));
            createClass.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            // Fill out Create Form
            var dept = driver.FindElement(By.Name("Department"));
            dept.Click();

            var math = driver.FindElement(By.Name("math"));
            math.Click();

            var number = driver.FindElement(By.Name("courseNumber"));
            number.Click();
            number.SendKeys("1010");

            var name = driver.FindElement(By.Name("courseName"));
            name.Click();
            name.SendKeys("Intro to Math");

            var credits = driver.FindElement(By.Name("credits"));
            credits.Click();
            credits.SendKeys(Keys.ArrowRight);

            var location = driver.FindElement(By.Name("location"));
            location.Click();
            location.SendKeys("Tracy Hall");

            var day = driver.FindElements(By.Name("day"));
            day[0].Click();
            day[2].Click();
            day[4].Click();

            var sTime = driver.FindElement(By.Name("start"));
            sTime.Click();
            sTime.SendKeys("1130AM");

            var eTime = driver.FindElement(By.Name("end"));
            eTime.Click();
            eTime.SendKeys("0120PM");

            var reg = driver.FindElement(By.Name("register"));
            reg.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            // Check if Class was created
            var home = driver.FindElement(By.Name("home"));
            home.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            int numCourses = driver.FindElements(By.ClassName("card")).Count;
            Assert.IsTrue(numCourses == 2);

            // Remove Class from database
            int id = _context.Class.OrderBy(c => c.ClassId).Last().ClassId;
            removeClass(id);

            // End Test
            driver.Quit();
        }
        public async void removeClass(int id)
        {
            Class? test = await _context.Class.FindAsync(id);
            _context.Class.Remove(test);
            await _context.SaveChangesAsync();
        }
    }
}