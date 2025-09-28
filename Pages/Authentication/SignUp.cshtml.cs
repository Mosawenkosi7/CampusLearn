using System.ComponentModel.DataAnnotations;
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn.Pages.Authentication
{
    public class SignUpModel : PageModel
    {

        private readonly AuthenticationServices _authService;

        public SignUpModel(AuthenticationServices authentication)
        {
            _authService = authentication;
        }



        [BindProperty]
        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Cellphone Number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Cellphone number must be 10 digits.")]
        public string PhoneNumber { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Personnel Number is required.")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "Personnel Number must be at least 6 digits.")]
        public string PersonnelNumber { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        // Regular expression to validate email domain.
        [RegularExpression(@".+@belgiumcampus\.ac\.za$", ErrorMessage = "Email must end with @belgiumcampus.ac.za.")]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Password must be confirmed")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";

        public void OnGet()
        {
        }

        public string ErrorMessage { get; set; } = "";  

        public string SuccessMessage { get; set; } = "";

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors in the form."; //check if required fields are filled
                return Page();
            }

            try
            {
                bool addNewUser = _authService.AddNewUser(PersonnelNumber, Email, Password, FirstName, LastName, PhoneNumber);

                if (addNewUser)
                {
                    SuccessMessage = "Profile has been created successfully!";
                    return RedirectToPage("/Authentication/LogIn");
                }
                else
                {
                    ErrorMessage = "Email or Personnel number already exists. Please use different credentials.";
                    return Page(); // Stay on the same page to show error
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while creating your account. Please try again.";
                Console.WriteLine($"SignUp error: {ex.Message}");
                return Page(); // Stay on the same page to show error
            }
        }
    }
}
