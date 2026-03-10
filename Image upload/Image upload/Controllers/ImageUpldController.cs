using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Web;

namespace Image_upload.Controllers
{
    
    public class ImageUpldController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        

        [HttpPost]
        public ActionResult Index(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + fileName);
                ViewBag.ImageUrl = "Uploads/" + fileName;
            }

            return View();
        }


    }
}
