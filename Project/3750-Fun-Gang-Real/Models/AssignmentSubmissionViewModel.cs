namespace Assignment_1.Models
{
    public class AssignmentSubmissionViewModel
    {
        public ClassAssignments? Assignment { get; set; }
        public IEnumerable<AssignmentSubmissions>? Submission { get; set; }
        public List<User>? User { get; set; }
        public int[] GradeBins = new int[11];//each index is the count of how many grades fall into that range, 0-9%,10-19% ... 80-89%,90-100%
        public int StudentBin { get; set; }
    }
}
