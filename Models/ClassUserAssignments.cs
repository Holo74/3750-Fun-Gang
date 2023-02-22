namespace Assignment_1.Models
{
    public class ClassUserAssignments
    {
        public Class Class { get; set; }
        public User User { get; set; }
        public List<ClassAssignments> Assignments { get; set; }

        public bool TeachesClass { get; set; }
    }
}