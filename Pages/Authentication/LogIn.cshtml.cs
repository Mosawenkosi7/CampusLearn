using System.ComponentModel.DataAnnotations;
using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn.Pages.Authentication
{
    public class LogInModel : PageModel
    {
        private readonly AuthenticationServices _authService;
        public LogInModel(AuthenticationServices authentication)
        {
            _authService = authentication;
        }
        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        // Regular expression to validate email domain.
        [RegularExpression(@".+@belgiumcampus\.ac\.za$", ErrorMessage = "Email must end with @belgiumcampus.ac.za.")]
        public string Email { get; set; } = "";

       

        [BindProperty]
        [Required(ErrorMessage ="Email is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return Page();
            }

            // Authentication logic here
            User? user = _authService.SignIn(Email, Password);

            if (user != null)
            {
                HttpContext.Session.SetString("personnelNumber", user.PersonnelNumber);
                HttpContext.Session.SetString("email", user.Email);
                HttpContext.Session.SetString("firstName", user.FirstName);
                HttpContext.Session.SetString("role", user.Role);

            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect Passowrd or Email";
            }
                //set the session variable 
                return RedirectToPage("/Student/Dashboard");

        }
    }
}
