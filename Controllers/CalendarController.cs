using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class CalendarController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object

        public CalendarController(Assignment_1Context context)
        {
            _context = context; // makes it so we can get the database at any time
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CalendarPage()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();

            List<EventCS3750> myEvents = new List<EventCS3750>();

            if (user[0].UserType == "Student")
            {
                var registrations = from r in _context.Registrations select r;
                var classes = registrations.Where(r => r.UserFK == UserID && r.IsRegistered == 1).Select(r => r.ClassFK).ToList();

                List<Class> classes2 = new List<Class>();
                foreach (var c in classes)
                {
                    var course = from cs in _context.Class select cs;
                    var course2 = course.Where(cs => cs.ClassId == c.Value).ToList();
                    classes2.Add(course2[0]);
                }

                foreach (var course in classes2)
                {
                    DateTime startDate = DateTime.Parse("2023-01-01T00:00:00");
                    for (int i = 0; i < 22; i++)
                    {
                        if (course.DaysOfWeek.Contains("Su"))
                        {
                            DateTime dummyDate = startDate;
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("M"))
                        {
                            DateTime dummyDate = startDate.AddDays(1);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;

                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("Tu"))
                        {
                            DateTime dummyDate = startDate.AddDays(2);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("W"))
                        {
                            DateTime dummyDate = startDate.AddDays(3);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("Th"))
                        {
                            DateTime dummyDate = startDate.AddDays(4);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("F"))
                        {
                            DateTime dummyDate = startDate.AddDays(5);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("Sa"))
                        {
                            DateTime dummyDate = startDate.AddDays(6);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }

                        startDate = startDate.AddDays(7);
                    }
                }
            }
            else if (user[0].UserType == "Teacher")
            {
                var classes = from c in _context.Class select c;
                var classes2 = classes.Where(c => c.UserId == UserID).ToList();

                foreach (var course in classes2)
                {
                    DateTime startDate = DateTime.Parse("2023-01-01T00:00:00");
                    for (int i = 0; i < 22; i++)
                    {
                        if (course.DaysOfWeek.Contains("Su"))
                        {
                            DateTime dummyDate = startDate;
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("M"))
                        {
                            DateTime dummyDate = startDate.AddDays(1);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;

                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("Tu"))
                        {
                            DateTime dummyDate = startDate.AddDays(2);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("W"))
                        {
                            DateTime dummyDate = startDate.AddDays(3);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("Th"))
                        {
                            DateTime dummyDate = startDate.AddDays(4);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("F"))
                        {
                            DateTime dummyDate = startDate.AddDays(5);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }
                        if (course.DaysOfWeek.Contains("Sa"))
                        {
                            DateTime dummyDate = startDate.AddDays(6);
                            EventCS3750 ev = new EventCS3750();

                            string start = course.StartTime.TimeOfDay.ToString();
                            string end = course.EndTime.TimeOfDay.ToString();

                            string startTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            string endTime = dummyDate.Year + "-0" + dummyDate.Month + "-";
                            if (dummyDate.Day < 10)
                            {
                                startTime += '0';
                                endTime += '0';
                                startTime += dummyDate.Day.ToString() + "T" + start;
                                endTime += dummyDate.Day.ToString() + "T" + end;
                            }
                            else
                            {
                                startTime += dummyDate.Day + "T" + start;
                                endTime += dummyDate.Day + "T" + end;
                            }

                            ev.StartTime = startTime;
                            ev.EndTime = endTime;
                            ev.Title = course.CourseName;

                            myEvents.Add(ev);
                        }

                        startDate = startDate.AddDays(7);
                    }
                }
            }

            return View(myEvents);
        }
    }
    public class EventCS3750
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
    }
}
