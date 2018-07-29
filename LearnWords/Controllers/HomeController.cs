using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnWords.Models;
using LearnWords.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LearnWords.Controllers
{
    public class HomeController : Controller
    {
        MongoRepository _repo;
        string _current_user;



        public HomeController(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, MongoRepository mr)
        {
            _repo = mr;
            try
            {
                _current_user = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            catch (NullReferenceException)
            {
                
            }
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            ViewData["UserEmail"] = _current_user;
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Browse()
        {
            _repo.Change(_current_user, "categories");
            ViewData["Message"] = "Your contact page.";
            return View(_repo.GetCategories());
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddCategory(CategoryModel cm)
        {
            _repo.AddCategory(cm);
            return RedirectToAction("Browse");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Play(string hash)
        {
            _repo.Change(_current_user, hash);
            return View(_repo.GetCollection());
        }

        [Authorize]
        [HttpGet]
        public IActionResult Explore(string hash)
        {
            _repo.Change(_current_user, hash);
            return View(_repo.GetCollection());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
