using System;
using System.Linq;
using System.Web.Mvc;
using Pharmacy.Models;

namespace Pharmacy.Controllers
{
    public class AccountController : Controller
    {
        private PharmacyContext db = new PharmacyContext();

        [HttpGet]
        public ActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Користувач з такою електронною поштою вже зареєстрований.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Email = user.Email.ToLower().Trim();

                    db.Users.Add(user);
                    db.SaveChanges();

                    Session["UserId"] = user.Id;
                    Session["UserEmail"] = user.Email;
                    Session["UserName"] = user.FirstName + " " + user.LastName;
                    Session["IsDoctor"] = user.IsDoctor;

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    var inner = ex;
                    while (inner.InnerException != null)
                    {
                        inner = inner.InnerException;
                    }

                    if (inner.Message.Contains("UNIQUE KEY constraint") ||
                        inner.Message.Contains("duplicate key"))
                    {
                        ModelState.AddModelError("Email", "Користувач з такою електронною поштою вже існує.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Помилка при збереженні: " + inner.Message);
                    }
                }
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "Введіть email і пароль.";
                return View();
            }

            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                
                Session["UserId"] = user.Id;
                Session["UserEmail"] = user.Email;
                Session["UserName"] = user.FirstName + " " + user.LastName;
                Session["IsDoctor"] = user.IsDoctor;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Невірна пошта або пароль.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Welcome", "Home");
        }
    }
}