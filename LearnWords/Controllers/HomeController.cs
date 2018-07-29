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

        GameLogic _gl;


        public HomeController(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, MongoRepository mr, GameLogic gl)
        {
            _repo = mr;
            _gl = gl;
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
            _repo.Init(_current_user);
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
        public IActionResult Play(string categoryhash)
        {
            _gl.Init(categoryhash, _repo);
            return View(_gl.GetNextWord());
        }

        [Authorize]
        [HttpGet]
        public IActionResult PlayNext(string wordhash, string categoryhash, bool result, int note)
        {
            WordModel next = _gl.GetNextWord();
            if (next == null)
            {
                return RedirectToAction("Browse");
            }
            return View("Play", next);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Explore(string categoryhash)
        {
            ViewData["categoryhash"] = categoryhash;
            return View(_repo.GetCollection(categoryhash));
        }

        [Authorize]
        [HttpGet]
        public IActionResult DeleteWord(string wordhash, string cathash)
        {
            _repo.DeleteWord(wordhash);
            return RedirectToAction("Explore", new { categoryhash = cathash });
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditWord(string wordhash, string cathash)
        {
            WordModel wordtoedit = _repo.GetWord(wordhash);
            return View(wordtoedit);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditWord(WordModel word)
        {
            _repo.EditWord(word);
            return RedirectToAction("Explore", new { categoryhash = word.Category });
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateWord(string categoryhash)
        {
            var word = new WordModel();
            word.Category = categoryhash;
            return View(word);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateWord(WordModel word)
        {
            _repo.AddWord(word);
            return RedirectToAction("Explore", new { categoryhash = word.Category });
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
