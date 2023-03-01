using Microsoft.AspNetCore.Mvc.Rendering;
namespace Assignment_1.Models

{
    public class ClassUserViewModel
    {
        public User viewUser { get; set; }
        public List<Class> classes { get; set; }

        //needed for getting classes a student is registered for?
        public List<Registrations> registrations { get; set; }
    }
}
