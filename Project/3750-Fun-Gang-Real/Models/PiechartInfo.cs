namespace Assignment_1.Models
{
    public class PiechartInfo
    {
        public PiechartInfo()
        {
            PiechartData = new List<PieChartNameandAmount>();
        }

        public string Title { get; set; }
        public System.Collections.Generic.List<PieChartNameandAmount> PiechartData { get; set; }
    }

    public struct PieChartNameandAmount
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }
}