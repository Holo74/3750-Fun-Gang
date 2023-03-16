using Assignment_1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Assignment_1.Controllers
{
    public class FileController : Controller
    {
        private readonly Assignment_1Context context;
        public FileController(Assignment_1Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return Redirect("/Uploader");
        }

        public async Task<IActionResult> Uploader(int? id)
        {
            // Replace with the actual id.
            id = 10;
            if(id is null || context.User is null)
            {
                return NotFound();
            }

            var user = await context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            // This is the important part.  Get the files.
            string[] filePaths = System.IO.Directory.GetFiles("wwwroot/Photos/", id.ToString() + ".*");
            if (filePaths.Length > 0)
            {
                // The wwwroot can't be included in the path that the page reads.  It starts in that folder.
                string modifiedFolder = filePaths[0].Substring(filePaths[0].IndexOf("/"));
                // Pass the filepath to the model
                Models.FileViewer file = new Models.FileViewer() { FilePath = modifiedFolder };
                return View(file);
            }

            return View(new Models.FileViewer());
        }

        [HttpPost]
        public async Task<IActionResult> Uploader()
        {
            string directory = "wwwroot/Photos";
            Directory.CreateDirectory(directory);
            // Get the file from this.  You don't actually need anything else except for this part.  
            IFormFile z = Request.Form.Files[0];
            // Getting file extension.  Might be an easier way to do this.
            string fileExtension = z.FileName.Substring(z.FileName.LastIndexOf('.'));

            // Need to replace 0 with the current instance of the user id
            using (Stream filestream = new FileStream(directory + "/" + "0" + fileExtension, FileMode.Create, FileAccess.Write))
            {
                // Saves the file to where ever the filestream was pointed to.
                z.CopyTo(filestream);
                // Won't save properly without this
                filestream.Close();
            }
            return View(new Models.FileViewer());
        }
    }
}
