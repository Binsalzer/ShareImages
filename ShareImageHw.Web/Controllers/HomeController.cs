using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Upload(IFormFile image, string password)
        {
            var fileName = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", $"{Guid.NewGuid}-{image.FileName}");


            using FileStream fs = new(fileName, FileMode.Create);
            image.CopyTo(fs);

            ImageRepo repo = new(_connectionString);
            var vm = new UploadViewModel { FileName=fileName, Password=password};

            repo.AddImage(new()
            {
                Path=fileName,
                Password=password
            });
            return View(vm);
        }
    }
}