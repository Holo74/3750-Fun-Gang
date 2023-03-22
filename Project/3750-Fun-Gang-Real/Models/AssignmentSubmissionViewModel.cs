namespace Assignment_1.Models
{
    public class AssignmentSubmissionViewModel
    {
        public ClassAssignments? Assignment { get; set; }
        public IEnumerable<AssignmentSubmissions>? Submission { get; set; }
        public List<User>? User { get; set; }
    }
}
