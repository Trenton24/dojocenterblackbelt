using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using exam.Models;

namespace exam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.SetInt32("id", 0);
            List<Users> AllUsers = dbContext.users.ToList();
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(Users user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                System.Console.WriteLine(user.Password);
                bool result =
                user.Password.Any(c => char.IsLetter(c)) &&
                user.Password.Any(c => char.IsDigit(c)) &&
                user.Password.Any(c => char.IsPunctuation(c)) ||
                user.Password.Any(c => char.IsSymbol(c));
                if(result == false)
                {
                    ModelState.AddModelError("Password", "Password must contain at least 1 number, digit, and symbol");
                    return View("Index");
                }
            
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                return Redirect("signin");
            }
            return View("Index");
        }

        [HttpGet]
        [Route("/signin")]
        public IActionResult signin()
        {
            return View("login");
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == user.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("login");
                }
                HttpContext.Session.SetInt32("id", userInDb.UserId);
                return Redirect("result");
            }
            return View("login");
        }

        [HttpGet]
        [Route("result")]
        public IActionResult Result()
        {
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}
            ViewBag.UserId = HttpContext.Session.GetInt32("id");
            List<Event> Events = dbContext.Event.Include(a => a.Attending).Include(b => b.Planner).OrderBy(a => a.Date).ToList();
            foreach(Event e in Events)
            {
                if(e.Date < DateTime.Now)
                {
                    var Wedding = dbContext.Event.FirstOrDefault(w => w.EventId == e.EventId);
                    dbContext.Remove(Wedding);
                    dbContext.SaveChanges();
                }
            }
            return View("dashboard", Events);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        [HttpGet]
        [Route("newEvent")]
        public IActionResult NewEvent()
        {
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}
            ViewBag.id = HttpContext.Session.GetInt32("id");
            return View("createEvent");
        }

        [HttpPost("CreateEvent")]
        public IActionResult CreateEvent(Event ev)
        {
            ViewBag.id = HttpContext.Session.GetInt32("id");
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}

            if(ModelState.IsValid)
            {
                dbContext.Event.Add(ev);
                dbContext.SaveChanges();
                ViewBag.eventId = ev.EventId;
                return Redirect($"event/{ev.EventId}");
            }
            return View("createEvent", ev);
        }

        [HttpGet("event/{EventId}")]
        public IActionResult ViewEvent(int eventid)
        {
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}
            ViewBag.UserId = HttpContext.Session.GetInt32("id");
            Event current = dbContext.Event.Include(p => p.Planner).Include(c => c.Attending).ThenInclude(attending => attending.Users).FirstOrDefault(w => w.EventId == eventid);
            return View(current);
        }

        [HttpGet("delete/{Eventid}")]
        public IActionResult DeleteEvent(int eventid)
        {
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}
            var Wedding = dbContext.Event.FirstOrDefault(w => w.EventId == eventid);
            dbContext.Event.Remove(Wedding);
            dbContext.SaveChanges();
            return Redirect("result");
        }

        [HttpGet("/unrsvpevent/{Eventid}")]
        public IActionResult UnRSVP(int eventid)
        {
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}
            int? id = HttpContext.Session.GetInt32("id"); 
            var rsvp = dbContext.RSVP.FirstOrDefault(r => r.EventId == eventid && r.UserId == (int)id);
            dbContext.RSVP.Remove(rsvp);
            dbContext.SaveChanges();
            return RedirectToAction("Result");
        }

        [HttpGet("/rsvpevent/{eventid}")]
        public IActionResult RSVP(int eventid)
        {
            var test = HttpContext.Session.GetInt32("id");
            if(test <= 0){return Redirect("/");}
            int? id = HttpContext.Session.GetInt32("id");
            System.Console.WriteLine(id);
            RSVP RSVP = new RSVP();
            RSVP.UserId = (int) id;
            RSVP.EventId = eventid;
            dbContext.Add(RSVP);
            dbContext.SaveChanges(); 
            return RedirectToAction("Result");
        }
    }
}
