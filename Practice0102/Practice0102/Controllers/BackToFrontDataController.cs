using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
namespace Practice0102.Controllers
{
    public class BackToFrontDataController : Controller
    {
        #region Single File Reading
        public IActionResult Index()
        {
            string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Controllers", "BackToFrontDataController.cs");
            string codeContent = System.IO.File.ReadAllText(filePath);
            ViewBag.CodeText = codeContent;
            return View();
        }
        #endregion

        #region Multi File Reading 
        public IActionResult NewIndex()
        {          
            string folderpath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Controllers");
            var files = Directory.GetFiles(folderpath, "*.cs");

            var codeFiles = new Dictionary<string, string>();
            foreach (var file in files)
            {
                string content = System.IO.File.ReadAllText(file);
                string fileName = System.IO.Path.GetFileName(file);
                codeFiles[fileName] = content;
            }               
            ViewBag.CodeFiles = codeFiles;
            return View();
        }
        #endregion
    }


}
