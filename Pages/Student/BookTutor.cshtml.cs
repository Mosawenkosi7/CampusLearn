using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn.Services;
using CampusLearn.Models;
using System.IO;
using System.Threading.Tasks;

namespace CampusLearn.Pages.Student
{
    public class BookTutorModel : PageModel
    {
        private readonly TutorService _tutorService;
        private readonly BookTutorService _bookTutorService;
        
        [BindProperty]
        public int AvailabilityId { get; set; }
        
        [BindProperty]
        public TutorAvailabilityView? Availability { get; set; }
        
        [BindProperty]
        public string StudentName { get; set; } = "";
        
        [BindProperty]
        public string StudentEmail { get; set; } = "";
        
        [BindProperty]
        public string Module { get; set; } = "";
        
        [BindProperty]
        public DateTime PreferredDate { get; set; }
        
        [BindProperty]
        public TimeSpan PreferredTime { get; set; }
        
        [BindProperty]
        public string PreferredTimeString { get; set; } = "";
        
        [BindProperty]
        public string SessionDuration { get; set; } = "";
        
        [BindProperty]
        public string Location { get; set; } = "";
        
        [BindProperty]
        public string BookingSummary { get; set; } = "";
        
        [BindProperty]
        public IFormFile? StudentResource1 { get; set; }
        
        [BindProperty]
        public IFormFile? StudentResource2 { get; set; }
        
        [BindProperty]
        public bool AgreeTerms { get; set; }
        
        public BookTutorModel(TutorService tutorService, BookTutorService bookTutorService)
        {
            _tutorService = tutorService;
            _bookTutorService = bookTutorService;
        }
        
        public IActionResult OnGet(int availabilityId)
        {
            // Check if user is logged in and is a student
            var personnelNumber = HttpContext.Session.GetString("personnelNumber");
            var role = HttpContext.Session.GetString("role");
            
            if (string.IsNullOrEmpty(personnelNumber) || role != "Student")
            {
                TempData["Error"] = "Please log in as a student to book a tutor.";
                return RedirectToPage("/Authentication/LogIn");
            }
            
            AvailabilityId = availabilityId;
            
            // Get availability details
            Availability = _tutorService.GetAvailabilityById(availabilityId);
            
            if (Availability == null)
            {
                TempData["Error"] = "The selected availability slot is no longer available.";
                return RedirectToPage("/Tutor/AllTutors");
            }
            
            if (Availability.IsBooked)
            {
                TempData["Error"] = "This time slot has already been booked.";
                return RedirectToPage("/Tutor/AllTutors");
            }
            
            // Get student information
            var student = _bookTutorService.GetStudentByPersonnelNumber(personnelNumber);
            if (student != null)
            {
                StudentName = $"{student.FirstName} {student.LastName}";
                StudentEmail = student.Email;
            }
            
            // Pre-populate form fields from availability
            Module = Availability.ModuleCode;
            PreferredDate = Availability.Available.Date; // Extract date from DATETIME
            PreferredTime = Availability.Available.TimeOfDay; // Extract time from DATETIME
            PreferredTimeString = Availability.Available.ToString("HH:mm"); // Format as HH:mm
            
            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if (!AgreeTerms)
            {
                TempData["Error"] = "You must agree to the terms and conditions.";
                return Page();
            }
            
            // Get logged-in student's personnel number
            var personnelNumber = HttpContext.Session.GetString("personnelNumber");
            if (string.IsNullOrEmpty(personnelNumber))
            {
                TempData["Error"] = "Please log in to complete the booking.";
                return RedirectToPage("/Authentication/LogIn");
            }
            
            try
            {
                // Handle file uploads
                string? document1 = null;
                string? document2 = null;
                
                if (StudentResource1 != null && StudentResource1.Length > 0)
                {
                    document1 = await SaveUploadedFile(StudentResource1);
                }
                
                if (StudentResource2 != null && StudentResource2.Length > 0)
                {
                    document2 = await SaveUploadedFile(StudentResource2);
                }
                
                // Create booking
                bool success = _bookTutorService.CreateBooking(
                    AvailabilityId, 
                    personnelNumber, 
                    Location, 
                    BookingSummary, 
                    document1, 
                    document2
                );
                
                if (success)
                {
                    TempData["Message"] = "Booking created successfully! You will receive a confirmation email shortly.";
                    return RedirectToPage("/Student/Dashboard");
                }
                else
                {
                    TempData["Error"] = "Failed to create booking. The time slot may no longer be available.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the booking. Please try again.";
                // Log the exception for debugging
                Console.WriteLine($"Booking error: {ex.Message}");
            }
            
            return Page();
        }
        
        private async Task<string?> SaveUploadedFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", "BookTutorResources");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            
            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return fileName;
        }
    }
}
