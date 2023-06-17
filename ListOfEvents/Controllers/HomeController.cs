using ListOfEvents.Models;
using ListOfEvents.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ListOfEvents.Controllers
{
    public class HomeController : Controller
    {

        IWebHostEnvironment app;

        private ApplicationDbContext db;

        public HomeController(ApplicationDbContext db, IWebHostEnvironment app)
        {
            this.db = db;
            this.app = app;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Events.ToListAsync());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user.Login);
                    return RedirectToAction("Index", "Home");
                } else
                {
                    ModelState.AddModelError("", "Некорректные логин или пароль");
                }
            }
            return View(model);
        }

        private async Task Authenticate(string login)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateModel createModel, IFormFile image, IFormFile refulations)
        {
            Console.WriteLine(image.FileName + " " + refulations.FileName);
            User user = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (user != null)
            {
                if (image != null)
                {
                    if (refulations != null)
                    {
                        string imagePath = "/Images/" + image.FileName;
                        string refulationsPath = "/Refulations/" + refulations.FileName;
                        using (var fileStream = new FileStream(app.WebRootPath + imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        using (var fileStream = new FileStream(app.WebRootPath + refulationsPath, FileMode.Create))
                        {
                            await refulations.CopyToAsync(fileStream);
                        }
                        await db.Events.AddAsync(new Event { EventDate = createModel.EventDate, EventOrganizer = createModel.EventOrganizer, EventName = createModel.EventName, EventDescription = createModel.EventDescription,
                            UserId = user.Id, RefulationsName = refulations.FileName, RefulationsPath = refulationsPath, ImageName = image.FileName, ImagePath = imagePath});
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        string imagePath = "/Images/" + image.FileName;
                        using (var fileStream = new FileStream(app.WebRootPath + imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        await db.Events.AddAsync(new Event
                        {
                            EventDate = createModel.EventDate,
                            EventOrganizer = createModel.EventOrganizer,
                            EventName = createModel.EventName,
                            UserId = user.Id,
                            RefulationsName = null,
                            RefulationsPath = null,
                            ImageName = image.FileName,
                            ImagePath = imagePath
                        });
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                } else
                {
                    if (refulations != null)
                    {
                        string refulationsPath = "/Refulations/" + refulations.FileName;
                        using (var fileStream = new FileStream(app.WebRootPath + refulationsPath, FileMode.Create))
                        {
                            await refulations.CopyToAsync(fileStream);
                        }
                        await db.Events.AddAsync(new Event
                        {
                            EventDate = createModel.EventDate,
                            EventOrganizer = createModel.EventOrganizer,
                            EventName = createModel.EventName,
                            UserId = user.Id,
                            RefulationsName = refulations.FileName,
                            RefulationsPath = refulationsPath,
                            ImageName = null,
                            ImagePath = null
                        });
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        await db.Events.AddAsync(new Event
                        {
                            EventDate = createModel.EventDate,
                            EventOrganizer = createModel.EventOrganizer,
                            EventName = createModel.EventName,
                            UserId = user.Id,
                            RefulationsName = null,
                            RefulationsPath = null,
                            ImageName = null,
                            ImagePath = null
                        });
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                }
            } else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EventDetails(int? eventId)
        {
            if(eventId != null)
            {
                Event eventDetail = await db.Events.FirstOrDefaultAsync(e => e.Id == eventId);
                if(eventDetail != null)
                {
                    ViewBag.EventName = eventDetail.EventName;
                    return View(eventDetail);
                } else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

       
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
