using CampusLearn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Pages
{
   /*/ public class ExtraCode
    {
        // Updated mock database to store users by their email.
        public static class MockDatabase
        {
            public static Dictionary<string, User> Users = new Dictionary<string, User>();
        }

        public class AccountController : Controller
        {
            [HttpGet]
            public IActionResult Register()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Register(RegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    if (MockDatabase.Users.ContainsKey(model.Email))
                    {
                        ModelState.AddModelError("Email", "An account with this email already exists.");
                        return View(model);
                    }

                    var newUser = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        AcademicProgram = model.AcademicProgram,
                        YearOfStudy = model.YearOfStudy,
                        Password = model.Password
                    };

                    MockDatabase.Users.Add(newUser.Email, newUser);

                    return RedirectToAction("Login");
                }

                return View(model);
            }

            [HttpGet]
            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Login(LoginViewModel model)
            {
                if (ModelState.IsValid)
                {
                    if (MockDatabase.Users.TryGetValue(model.Email, out var user) && user.Password == model.Password)
                    {
                        return RedirectToAction("Welcome", "Home", new { username = user.FirstName });
                    }

                    ModelState.AddModelError("", "Invalid email or password.");
                }

                return View(model);
            }
        }
    }
}
    }
}
    }/*/
}
