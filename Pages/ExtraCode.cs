using CampusLearn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Pages
{
    public class ExtraCode
    {
        // Updated mock database to store users by their email.
        public static class MockDatabase
        {
            public static Dictionary<string, User> Users = new Dictionary<string, User>();
        }

        // Updated ViewModel for the registration form with all new fields.
        public class RegisterViewModel
        {
            [Required(ErrorMessage = "First Name is required.")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required.")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Academic Program is required.")]
            public string AcademicProgram { get; set; }

            [Required(ErrorMessage = "Year of Study is required.")]
            public string YearOfStudy { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required.")]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }
        }

        // Updated ViewModel for the login form to use Email instead of Username.
        public class LoginViewModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
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
    }
}
