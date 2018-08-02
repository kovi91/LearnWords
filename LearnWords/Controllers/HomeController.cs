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
using System.IO;

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
        [HttpPost]
        public IActionResult MultiPlayCustom(string [] values)
        {
            _gl.Init(values.ToList(), _repo, 25);
            return View("Play", _gl.GetNextWord());
        }

        [Authorize]
        [HttpGet]
        public IActionResult MultiPlayRandom()
        {
            _gl.Init(_repo, false,  25);
            return View("Play", _gl.GetNextWord());
        }

        [Authorize]
        [HttpGet]
        public IActionResult MultiPlayWeak()
        {
            _gl.Init(_repo, true,  25);
            return View("Play", _gl.GetNextWord());
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
        public IActionResult PlayNext(string wordhash, string categoryhash, int result, int note)
        {
            _gl.AddResult(wordhash, result, note);
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
        public IActionResult DeleteWords(string categoryhash)
        {
            _repo.DeleteAll(categoryhash);
            return RedirectToAction("Explore", categoryhash);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Learn(string categoryhash)
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
        public  IActionResult CreateWord(string categoryhash)
        {
            var word = new WordModel();
            word.Category = categoryhash;
            return View(word);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateWord(WordModel word)
        {
            await _repo.AddWord(word);
            return RedirectToAction("Explore", new { categoryhash = word.Category });
        }

        [Authorize]
        [HttpGet]
        public IActionResult ImportWords(string categoryhash)
        {
            return View("ImportWords", categoryhash);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ImportWords(string categoryhash, IFormFile Zip)
        {
            await _repo.ImportWords(categoryhash, Zip);
            return RedirectToAction("Explore", "Home", categoryhash);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
