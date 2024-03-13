using Microsoft.AspNetCore.Mvc;
using ShareImageHw.Data;
using ShareImageHw.Web.Models;
using System.Text.Json;

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
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", fileName);


            using FileStream fs = new(fullPath, FileMode.Create);
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

        public IActionResult ViewImage(string password,int id)
        {
            var accessableImages = HttpContext.Session.Get<List<int>>("Ids");
            if(accessableImages==null)
            {
                accessableImages = new List<int>();
            }


            ImageRepo repo = new(_connectionString);
            var currentImage = repo.GetImageById(id);
            var vm = new ViewImageViewModel();

            if(password==null)
            {
                vm.Image = new() { Id = id };
                return View(vm);
            }

            if (password==currentImage.Password)
            {
                accessableImages.Add(id);
                HttpContext.Session.Set("Ids", accessableImages);
            }

            if (accessableImages.Contains(id))
            {
                vm.Image = currentImage;
                vm.Authenticated = true;
                repo.UpdateViewCount(id, currentImage.ViewCount+1);
            }
            else
            {
                vm.Image = new() { Id = id };
                vm.FalseAttemt = true;
            }

            return View(vm);
        }
    }


    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}