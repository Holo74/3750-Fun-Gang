namespace Assignment_1.Models
{
	public class StudentAssignmentListViewModel
	{
		public List<AssignmentSubmissionViewModel> Assignments { get; set; }
		public Class Class { get; set; }
		public User User { get; set; }
		public bool TeachesClass { get; set; }
		public int[] array = new int[12]; 
		public System.Collections.Generic.List<PieChartNameandAmount> PiechartData { get; set; }

    }
}
