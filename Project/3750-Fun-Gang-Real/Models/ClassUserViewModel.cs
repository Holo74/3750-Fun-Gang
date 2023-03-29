using Microsoft.AspNetCore.Mvc.Rendering;
namespace Assignment_1.Models

{
    public class ClassUserViewModel
    {
        public User viewUser { get; set; }
        public List<Class> classes { get; set; }
        //public List<ClassAssignments> assignments { get; set; }
        public List<TODOitem> todoitems { get; set; }
        public List<string> notifications { get; set; }

        public string myFilePath { get; set; }

        //needed for getting classes a student is registered for?
        public List<Registrations> registrations { get; set; }
    }

    public class TODOitem       
    {
        public int CourseNumber { get; set; }
        public string AssignmentTitle { get; set; }
        public int ID { get; set; }
        public DateTime? dueDate { get; set; }
        public DateTime? dueTime { get; set; }
    }
}
