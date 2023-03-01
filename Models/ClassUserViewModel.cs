using Microsoft.AspNetCore.Mvc.Rendering;
namespace Assignment_1.Models

{
    public class ClassUserViewModel
    {
        public User viewUser { get; set; }
        public List<Class> classes { get; set; }

        public string myFilePath { get; set; }
    }
}
