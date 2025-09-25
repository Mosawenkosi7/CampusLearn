using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn.Pages.Authentication
{
    public class SignUpModel : PageModel
    {
        [BindProperty]
        [Required]
        public string FirstName { get; set; }

        [BindProperty]
        [Required]
        public string LastName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Cellphone Number is required.")]
        public string CellphoneNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Personnel Number is required.")]
        // Regular expression to enforce "AD" or "ST" prefix.
        [RegularExpression(@"^(AD|ST)\d{4,}$", ErrorMessage = "Personnel Number must start with 'AD' or 'ST' followed by digits.")]
        public string PersonnelNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        // Regular expression to validate email domain.
        [RegularExpression(@"^[\w\.-]+@(student\.com|campus\.edu)$", ErrorMessage = "Email must end in @student.com or @campus.edu.")]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Registration logic here
            return RedirectToPage("/Index");
        }
    }
}
