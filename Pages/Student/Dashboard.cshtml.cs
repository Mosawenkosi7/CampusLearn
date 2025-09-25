using CampusLearn.Repositories;
using CampusLearn.Models; 
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn.Pages.Student
{
    public class DashboardModel : PageModel
    {
        private readonly StudentDashboardService _studentDashboardService;

        //SET THE ID
       

        public DashboardModel(StudentDashboardService studentDashboardService)
        {
            _studentDashboardService = studentDashboardService;
        }


        //Number of sessions booked 
        public int totalSessions { get; set; }
        public int totalUpcomingSessions { get; set; }
        public int completedSessions { get; set; }

        public int pendingSessions { get; set; }
        //list to store bookings (paged)
        public List<AppointmentTable> appointmentTables = new List<AppointmentTable>();

        // pagination
        public int PageSize { get; } = 5;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }



        //store the booking Id 
        [BindProperty]
        public Booking Booking {get;set;}

        public List<LearningResourceCard> learningResources = new List<LearningResourceCard>();

        public void OnGet(int p = 1)
        {
            var studentId = HttpContext.Session.GetString("personnelNumber");
            if(string.IsNullOrEmpty(studentId))
            {
                RedirectToPage("/Authentication/LogIn");
                return; // Important: exit early if no student ID
            }

            // all data ordered from service - studentId is guaranteed non-null here
            var allAppointments = _studentDashboardService.GetAppointmentTables(studentId);

            // pagination setup
            CurrentPage = p < 1 ? 1 : p;
            TotalPages = (int)Math.Ceiling(allAppointments.Count / (double)PageSize);
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }

            appointmentTables = allAppointments
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();


            learningResources = _studentDashboardService.GetLearningResources(studentId);

            //set the total sessions booked 
            totalSessions = _studentDashboardService.BookedSessionsCount(studentId);

            totalUpcomingSessions = _studentDashboardService.UpcomingSessionsCount(studentId);

            completedSessions = _studentDashboardService.CompletedSessions(studentId);

            pendingSessions = _studentDashboardService.PendingSessionsCount(studentId);

        }


        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }
        public IActionResult OnPostCancelSession()
        {
            bool success = _studentDashboardService.CancelQuery(Booking.BookingId, Booking.Status);
            if (!success)
            {
                ErrorMessage = "Appointment marked with Complete Can not be deleted";
                return RedirectToPage("/Student/Dashboard");
            }

            SuccessMessage = "Appointment successfully cancelled";
            return RedirectToPage("/Student/Dashboard");

        }



    }


    
}
