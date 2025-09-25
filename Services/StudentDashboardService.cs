using CampusLearn.Repositories;

namespace CampusLearn.Services
{
    public class StudentDashboardService
    {
        private readonly StudentDashboardRepository _studentDashboardRepository;

        public StudentDashboardService(StudentDashboardRepository studentDashboardRepository)
        {
            _studentDashboardRepository = studentDashboardRepository;
        }

        //method to get the bookings for a student and apply business rules 
        public List<AppointmentTable> GetAppointmentTables(string studentId)
        {
            // get the data from Repository 
            var appointmentTables = _studentDashboardRepository.GetBookings(studentId);

            var now = DateTime.Now;
            var upcoming = appointmentTables
                .Where(a => a.Booking.DateBooked >= now)
                .OrderBy(a => a.Booking.DateBooked);

            var past = appointmentTables
                .Where(a => a.Booking.DateBooked < now)
                .OrderByDescending(a => a.Booking.DateBooked);

            return upcoming.Concat(past).ToList();
        }



        //method that deletes row in database
        public bool CancelQuery(int bookingId, string status) //returns true or false(
        {
            if (status == "Active" || status == "Pending")
            {
                _studentDashboardRepository.CancelBooking(bookingId);
                return true;
            }
            return false;
        }

        //create method that will count number of sessions booked
        public int BookedSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetBookedSessionsCount(studentId);
            return totalSessionsCount;
        }

        //create method that will count number of sessions booked
        public int UpcomingSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetUpComingSessions(studentId);
            return totalSessionsCount;
        }

        //create method that will count completed sessions
        public int CompletedSessions(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetCompletedSessions(studentId);
            return totalSessionsCount;
        }

    }
}
