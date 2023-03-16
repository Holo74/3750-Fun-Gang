using Assignment_1.Data;
using Microsoft.AspNetCore.Mvc;
using Assignment_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_1.Controllers
{
    public class PiechartDemoController : Controller
    {
        private readonly Assignment_1Context context;
        public PiechartDemoController(Assignment_1Context context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            PiechartInfo dummyData = new PiechartInfo() { Title = "Hello world" };
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "Testing", Amount = 10 });
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "J", Amount = 10 });
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "A", Amount = 1 });
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "C", Amount = 20 });
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "B", Amount = 15 });
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "D", Amount = 5 });
            dummyData.PiechartData.Add(new PieChartNameandAmount() { Name = "E", Amount = 30 });
            return View(dummyData);
        }
    }
}