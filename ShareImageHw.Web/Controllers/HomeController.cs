using AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ShareImageHw.Data;
using ShareImageHw.Web.Models;
using System.Diagnostics;

namespace ShareImageHw.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=ImageLoading;Integrated Security=True;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload(IFormFile imageFile, string password)
        {
            var fileName = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", $"{Guid.NewGuid}-{imageFile.FileName}");


            using FileStream fs = new(fileName, FileMode.Create);
            imageFile.CopyTo(fs);

            ImageRepo repo = new(_connectionString);

            var id = repo.AddImage(new()
            {
                Path = fileName,
                Password = password
            });

            var vm = new UploadViewModel { Id = id, Password = password };
            return View(vm);
        }

        public IActionResult ViewImage(int Id)
        {
            ImageRepo repo = new(_connectionString);
            var vm = new ViewImageViewModel { Image=repo.GetImageById(Id)};

            return View(vm);
        }
    }
}